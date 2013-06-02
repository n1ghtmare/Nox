using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Moq;
using NUnit.Framework;
using Nox.Providers;
using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.ConductorTests
{
    public class Execute
    {
        [Test]
        public void Query_CallsProviderCreateConnection()
        {
            // Arrange
            var conductor = TestableConductor.Create();

            // Act
            conductor.Execute<TestEntity1>(TestableConductor.Query).ToList();

            // Assert
            conductor.MockProvider
                     .Verify(x => x.CreateConnection(), Times.Once());
        }

        [Test]
        public void Query_CallsProviderCreateCommand()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockConnection = new Mock<IDbConnection>();

            conductor.MockProvider
                     .Setup(x => x.CreateConnection())
                     .Returns(mockConnection.Object);

            // Act
            conductor.Execute<TestEntity1>(TestableConductor.Query).ToList();

            // Assert
            conductor.MockProvider
                     .Verify(x => x.CreateCommand(TestableConductor.Query, mockConnection.Object, (CommandType) 0),
                             Times.Once());
        }

        [Test]
        public void QueryWithParameters_CallsProviderCreateParameters()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var parameters = new {TestPropertyString = "TEST_STRING", TestPropertyInt = 123};


            // Act
            conductor.Execute<TestEntity1>(TestableConductor.QueryWithParameters, parameters).ToList();

            // Assert
            conductor.MockProvider
                     .Verify(x => x.CreateParameters(
                         It.Is<Dictionary<string, object>>(
                             d => (string)  d["TestPropertyString"] == "TEST_STRING" &&
                                  (int)     d["TestPropertyInt"] == 123)),
                             Times.Once());
        }
        
        [Test]
        public void QueryWithParameters_CallsProviderCommandAddParameterForEachProvidedParameter()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var parameters = new { TestPropertyString = "TEST_STRING", TestPropertyInt = 123 };
            var mockCommand = new Mock<IDbCommand>();
            var mockParameterCollection = new Mock<IDataParameterCollection>();

            mockCommand.Setup(x => x.Parameters)
                       .Returns(mockParameterCollection.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateParameters(It.IsAny<IDictionary<string, object>>()))
                     .Returns(new List<IDbDataParameter>
                     {
                         new SqlParameter("@TestPropertyString", "TEST_STRING"),
                         new SqlParameter("@TestPropertyInt", 123)
                     });

            // Act
            conductor.Execute<TestEntity1>(TestableConductor.QueryWithParameters, parameters).ToList();

            // Assert
            mockParameterCollection
                .Verify(x => x.Add(It.IsAny<IDataParameter>()),
                        Times.Exactly(2));
        }

        [Test]
        public void QueryWithParametersOfTypeDictionary_CallsProviderCreateParameters()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var parameters = new Dictionary<string, object>
            {
                {"TestPropertyString", "TEST_STRING"},
                {"TestPropertyInt", 123}
            };

            // Act
            conductor.Execute<TestEntity1>(TestableConductor.QueryWithParameters, parameters).ToList();

            // Assert
            conductor.MockProvider
                     .Verify(x => x.CreateParameters(
                         It.Is<Dictionary<string, object>>(
                             d => (string)d["TestPropertyString"] == "TEST_STRING" &&
                                  (int)d["TestPropertyInt"] == 123)),
                             Times.Once());
        }

        [Test]
        public void Query_CallsProviderConnectionOpen()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockConnection = new Mock<IDbConnection>();

            conductor.MockProvider
                     .Setup(x => x.CreateConnection())
                     .Returns(mockConnection.Object);

            // Act
            conductor.Execute<TestEntity1>(TestableConductor.Query).ToList();

            // Assert
            mockConnection.Verify(x => x.Open(), Times.Once());
        }

        [Test]
        public void Query_CallsProviderCommandExecuteReader()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockCommand = new Mock<IDbCommand>();

            conductor.MockProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType)0))
               .Returns(mockCommand.Object);

            // Act
            conductor.Execute<TestEntity1>(TestableConductor.Query).ToList();

            // Assert
            mockCommand.Verify(x => x.ExecuteReader(), Times.Once());
        }

        [Test]
        public void QueryThatFindsResults_ReturnsAListOfDynamicResultsWithCorrectCount()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockDataReader = TestableConductor.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            // Act
            IEnumerable<dynamic> results = conductor.Execute(TestableConductor.Query).ToList();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("TestResult", results.First().TestPropertyString);
        }

        [Test]
        public void QueryReturningMoreColumnsThanThePropertiesInTheType_ReturnsCorrectlyMappedType()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockDataReader = TestableConductor.CreateDataReader();
            var readToggle = true;

            mockDataReader.Setup(x => x.Read()).Returns(() => readToggle).Callback(() => readToggle = false);
            mockDataReader.Setup(x => x.FieldCount).Returns(4);

            mockDataReader.Setup(x => x.GetName(3)).Returns("TestPropertyNonExistant");
            mockDataReader.Setup(x => x[3]).Returns("TestValueThatIsIrrelevant");

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            // Act
            TestEntity1 testEntity = conductor.Execute<TestEntity1>(TestableConductor.Query).ToList().First();

            // Assert
            Assert.AreEqual(DateTime.Today, testEntity.TestPropertyDateTime);
            Assert.AreEqual(1, testEntity.TestPropertyInt);
            Assert.AreEqual("TestResult", testEntity.TestPropertyString);
        }

        [Test]
        public void QueryReturningADBValueNullFor2Columns_MapsItToTheCorrectDefaultValue()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockDataReader = TestableConductor.CreateDataReader();
            var readToggle = true;

            mockDataReader.Setup(x => x.Read()).Returns(() => readToggle).Callback(() => readToggle = false);
            mockDataReader.Setup(x => x.FieldCount).Returns(3);

            mockDataReader.Setup(x => x.GetName(0)).Returns("TestPropertyString");
            mockDataReader.Setup(x => x[0]).Returns(DBNull.Value);

            mockDataReader.Setup(x => x.GetName(1)).Returns("TestPropertyInt");
            mockDataReader.Setup(x => x[1]).Returns(DBNull.Value);

            mockDataReader.Setup(x => x.GetName(2)).Returns("TestPropertyDate");
            mockDataReader.Setup(x => x[2]).Returns(DBNull.Value);

            mockDataReader.Setup(x => x.GetName(3)).Returns("TestPropertyIntNullable");
            mockDataReader.Setup(x => x[3]).Returns(DBNull.Value);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType)0))
                     .Returns(mockCommand.Object);

            // Act
            TestEntity5 testEntity = conductor.Execute<TestEntity5>(TestableConductor.Query).ToList().First();

            // Assert
            Assert.IsNullOrEmpty(testEntity.TestPropertyString);
            Assert.AreEqual(0, testEntity.TestPropertyInt);
            Assert.AreEqual(DateTime.MinValue, testEntity.TestPropertyDateTime);
            Assert.IsNull(testEntity.TestPropertyIntNullable);
        }

        [Test]
        public void QueryAndType_ReturnsCorrectlyMappedType()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockDataReader = TestableConductor.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            // Act
            TestEntity1 testEntity = conductor.Execute<TestEntity1>(TestableConductor.Query).ToList().First();

            // Assert
            Assert.AreEqual(DateTime.Today, testEntity.TestPropertyDateTime);
            Assert.AreEqual(1, testEntity.TestPropertyInt);
            Assert.AreEqual("TestResult", testEntity.TestPropertyString);
        }

        [Test]
        public void QueryAndAnInvalidType_ThrowsAnException()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockDataReader = TestableConductor.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            // Act
            var results =
                Assert.Throws<Exception>(
                    () => conductor.Execute<int>(query: TestableConductor.Query).ToList());

            // Assert
            StringAssert.StartsWith(
                "Can't map the results to the provided type, you can try to use dynamic as return type.",
                results.Message);
        }

        [Test]
        public void Scenario_Behavior()
        {
            // Arrange
            var sqlServerProvider = new SqlServerProvider();
            var conductor = new Conductor(sqlServerProvider);

            // Act
            var results = conductor.Execute<Test>("select * from Test_1").ToList();

            // Assert

        }
    }

    class Test
    {
        public int Id { get; set; }
        public string TestString { get; set; }
    }
}
