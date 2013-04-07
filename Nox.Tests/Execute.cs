using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

namespace Nox.Tests
{
    public class Execute
    {
        [Test]
        public void Query_CallsNoxProviderCreateConnection()
        {
            // Arrange
            var nox = TestableNox.Create();
            
            // Act
            nox.Execute<TestEntity>(TestableNox.Query).ToList();

            // Assert
            nox.MockNoxProvider
                .Verify(x => x.CreateConnection(), Times.Once());
        }

        [Test]
        public void Query_CallsNoxProviderCreateCommand()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider
               .Setup(x => x.CreateConnection())
               .Returns(mockConnection.Object);

            // Act
            nox.Execute<TestEntity>(TestableNox.Query).ToList();

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateCommand(TestableNox.Query, mockConnection.Object, false),
                       Times.Once());
        }

        [Test]
        public void QueryWithParameters_CallsNoxProviderCreateParameters()
        {
            // Arrange
            var nox = TestableNox.Create();
            var parameters = new {FirstName = "John", LastName = "Doe"};


            // Act
            nox.Execute<TestEntity>(TestableNox.QueryWithParameters, parameters).ToList();

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateParameters(parameters),
                       Times.Once());
        }

        [Test]
        public void QueryWithParameters_CallsNoxProviderCommandAddParameterForEachProvidedParameter()
        {
            // Arrange
            var nox = TestableNox.Create();
            var parameters = new { TestFirstParameter = "FirstParameterValue", TestSecondParameter = "SecondParameterValue" };
            var mockCommand = new Mock<IDbCommand>();
            var mockParameterCollection = new Mock<IDataParameterCollection>();

            mockCommand.Setup(x => x.Parameters)
                       .Returns(mockParameterCollection.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), false))
               .Returns(mockCommand.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateParameters(It.IsAny<object>()))
               .Returns(new List<IDbDataParameter>
                   {
                       new SqlParameter("@TestFirstParameter", "FirstParameterValue"), 
                       new SqlParameter("@TestSecondParameter", "SecondParameterValue")
                   });
            
            // Act
            nox.Execute<TestEntity>(TestableNox.QueryWithParameters, parameters).ToList();

            // Assert
            mockParameterCollection
                .Verify(x => x.Add(It.IsAny<IDataParameter>()),
                Times.Exactly(2));
        }

        [Test]
        public void Query_CallsNoxProviderConnectionOpen()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider
               .Setup(x => x.CreateConnection())
               .Returns(mockConnection.Object);

            // Act
            nox.Execute<TestEntity>(TestableNox.Query).ToList();

            // Assert
            mockConnection.Verify(x => x.Open(), Times.Once());
        }

        [Test]
        public void Query_CallsNoxProviderCommandExecuteReader()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockCommand = new Mock<IDbCommand>();

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), false))
               .Returns(mockCommand.Object);
            
            // Act
            nox.Execute<TestEntity>(TestableNox.Query).ToList();

            // Assert
            mockCommand.Verify(x => x.ExecuteReader(), Times.Once());
        }

        [Test]
        public void QueryThatFindsResults_ReturnsAListOfDynamicResultsWithCorrectCount()
        {
            // Arrange
            var nox = TestableNox.Create(); 
            var mockDataReader = TestableNox.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), false))
               .Returns(mockCommand.Object);

            // Act
            IEnumerable<dynamic> results = nox.Execute(TestableNox.Query).ToList();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("TestResult", results.First().TestPropertyString);
        }

        [Test]
        public void QueryReturningMoreColumnsThanThePropertiesInTheType_ReturnsCorrectlyMappedType()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockDataReader = TestableNox.CreateDataReader();
            var readToggle = true;

            mockDataReader.Setup(x => x.Read()).Returns(() => readToggle).Callback(() => readToggle = false);
            mockDataReader.Setup(x => x.FieldCount).Returns(4);      
            
            mockDataReader.Setup(x => x.GetName(3)).Returns("TestPropertyNonExistant");
            mockDataReader.Setup(x => x[3]).Returns("TestValueThatIsIrrelevant");

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), false))
               .Returns(mockCommand.Object);

            // Act
            TestEntity testEntity = nox.Execute<TestEntity>(TestableNox.Query).ToList().First();

            // Assert
            Assert.AreEqual(DateTime.Today, testEntity.TestPropertyDateTime);
            Assert.AreEqual(1, testEntity.TestPropertyInt);
            Assert.AreEqual("TestResult", testEntity.TestPropertyString);
        }

        [Test]
        public void QueryAndType_ReturnsCorrectlyMappedType()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockDataReader = TestableNox.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), false))
               .Returns(mockCommand.Object);

            // Act
            TestEntity testEntity = nox.Execute<TestEntity>(TestableNox.Query).ToList().First();

            // Assert
            Assert.AreEqual(DateTime.Today, testEntity.TestPropertyDateTime);
            Assert.AreEqual(1, testEntity.TestPropertyInt);
            Assert.AreEqual("TestResult", testEntity.TestPropertyString);
        }

        [Test]
        public void QueryAndAnInvalidType_ThrowsAnException()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockDataReader = TestableNox.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), false))
               .Returns(mockCommand.Object);

            // Act
            var results = Assert.Throws<Exception>(() => nox.Execute<int>(query: TestableNox.Query).ToList());

            // Assert
            StringAssert.StartsWith(
                "Can't map the results to the provided type, you can try to use dynamic as return type.",
                results.Message);
        }
    }
}
