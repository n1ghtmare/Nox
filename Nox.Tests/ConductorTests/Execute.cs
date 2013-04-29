using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.ConductorTests
{
    public class Execute
    {
        [Test]
        public void Query_CallsNoxProviderCreateConnection()
        {
            // Arrange
            var conductor = TestableConductor.Create();

            // Act
            Enumerable.ToList<TestEntity1>(conductor.Execute<TestEntity1>(TestableConductor.Query));

            // Assert
            conductor.MockNoxProvider
                     .Verify(x => x.CreateConnection(), Times.Once());
        }

        [Test]
        public void Query_CallsNoxProviderCreateCommand()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockConnection = new Mock<IDbConnection>();

            conductor.MockNoxProvider
                     .Setup(x => x.CreateConnection())
                     .Returns(mockConnection.Object);

            // Act
            Enumerable.ToList<TestEntity1>(conductor.Execute<TestEntity1>(TestableConductor.Query));

            // Assert
            conductor.MockNoxProvider
                     .Verify(x => x.CreateCommand(TestableConductor.Query, mockConnection.Object, (CommandType) 0),
                             Times.Once());
        }

        [Test]
        public void QueryWithParameters_CallsNoxProviderCreateParameters()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var parameters = new {FirstName = "John", LastName = "Doe"};


            // Act
            Enumerable.ToList<TestEntity1>(conductor.Execute<TestEntity1>(TestableConductor.QueryWithParameters, parameters));

            // Assert
            conductor.MockNoxProvider
               .Verify(x => x.CreateParameters(parameters),
                       Times.Once());
        }

        [Test]
        public void QueryWithParameters_CallsNoxProviderCommandAddParameterForEachProvidedParameter()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var parameters =
                new {TestFirstParameter = "FirstParameterValue", TestSecondParameter = "SecondParameterValue"};
            var mockCommand = new Mock<IDbCommand>();
            var mockParameterCollection = new Mock<IDataParameterCollection>();

            mockCommand.Setup(x => x.Parameters)
                       .Returns(mockParameterCollection.Object);

            conductor.MockNoxProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            conductor.MockNoxProvider
                     .Setup(x => x.CreateParameters(It.IsAny<object>()))
                     .Returns(new List<IDbDataParameter>
                     {
                         new SqlParameter("@TestFirstParameter", "FirstParameterValue"),
                         new SqlParameter("@TestSecondParameter", "SecondParameterValue")
                     });

            // Act
            Enumerable.ToList<TestEntity1>(conductor.Execute<TestEntity1>(TestableConductor.QueryWithParameters,
                                                                          parameters));

            // Assert
            mockParameterCollection
                .Verify(x => x.Add(It.IsAny<IDataParameter>()),
                        Times.Exactly(2));
        }

        [Test]
        public void Query_CallsNoxProviderConnectionOpen()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockConnection = new Mock<IDbConnection>();

            conductor.MockNoxProvider
                     .Setup(x => x.CreateConnection())
                     .Returns(mockConnection.Object);

            // Act
            Enumerable.ToList<TestEntity1>(conductor.Execute<TestEntity1>(TestableConductor.Query));

            // Assert
            mockConnection.Verify(x => x.Open(), Times.Once());
        }

        [Test]
        public void Query_CallsNoxProviderCommandExecuteReader()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockCommand = new Mock<IDbCommand>();

            conductor.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType)0))
               .Returns(mockCommand.Object);

            // Act
            Enumerable.ToList<TestEntity1>(conductor.Execute<TestEntity1>(TestableConductor.Query));

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

            conductor.MockNoxProvider
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

            conductor.MockNoxProvider
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
        public void QueryAndType_ReturnsCorrectlyMappedType()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockDataReader = TestableConductor.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            conductor.MockNoxProvider
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

            conductor.MockNoxProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            // Act
            var results =
                Assert.Throws<Exception>(
                    () => Enumerable.ToList<int>(conductor.Execute<int>(query: TestableConductor.Query)));

            // Assert
            StringAssert.StartsWith(
                "Can't map the results to the provided type, you can try to use dynamic as return type.",
                results.Message);
        }
    }
}
