using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

namespace Nox.Tests
{
    [TestFixture]
    public class ExecuteScalar
    {
        [Test]
        public void QueryScalar_CallsNoxProviderCreateConnection()
        {
            // Arrange
            var nox = TestableNox.Create();

            // Act
            nox.ExecuteScalar<int>(TestableNox.QueryScalar);

            // Assert
            nox.MockNoxProvider
                .Verify(x => x.CreateConnection(), Times.Once());
        }

        [Test]
        public void QueryScalar_CallsNoxProviderCreateCommand()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider
               .Setup(x => x.CreateConnection())
               .Returns(mockConnection.Object);

            // Act
            nox.ExecuteScalar<int>(TestableNox.QueryScalar);

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateCommand(TestableNox.QueryScalar, mockConnection.Object, (CommandType)0),
                       Times.Once());
        }

        [Test]
        public void QueryScalarWithParameters_CallsNoxProviderCreateParameters()
        {
            // Arrange
            var nox = TestableNox.Create();
            var parameters = new { FirstName = "John", LastName = "Doe" };


            // Act
            nox.ExecuteScalar<int>(TestableNox.QueryScalarWithParameters, parameters);

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateParameters(parameters),
                       Times.Once());
        }

        [Test]
        public void QueryScalarWithParameters_CallsNoxProviderCommandAddParameterForEachProvidedParameter()
        {
            // Arrange
            var nox = TestableNox.Create();
            var parameters = new { TestFirstParameter = "FirstParameterValue", TestSecondParameter = "SecondParameterValue" };
            var mockCommand = new Mock<IDbCommand>();
            var mockParameterCollection = new Mock<IDataParameterCollection>();

            mockCommand.Setup(x => x.Parameters).Returns(mockParameterCollection.Object);
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(0);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType)0))
               .Returns(mockCommand.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateParameters(It.IsAny<object>()))
               .Returns(new List<IDbDataParameter>
                   {
                       new SqlParameter("@TestFirstParameter", "FirstParameterValue"), 
                       new SqlParameter("@TestSecondParameter", "SecondParameterValue")
                   });

            // Act
            nox.ExecuteScalar<int>(TestableNox.QueryScalarWithParameters, parameters);

            // Assert
            mockParameterCollection
                .Verify(x => x.Add(It.IsAny<IDataParameter>()),
                Times.Exactly(2));
        }

        [Test]
        public void QueryScalarWithParameters_CallsNoxProviderConnectionOpen()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            // Act
            nox.ExecuteScalar<int>(TestableNox.QueryScalar);

            // Assert
            mockConnection.Verify(x => x.Open(), Times.Once());
        }

        [Test]
        public void QueryScalar_CallsNoxProviderCommandExecuteScalar()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockCommand = new Mock<IDbCommand>();
            
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(0);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
               .Returns(mockCommand.Object);

            // Act
            nox.ExecuteScalar<int>(TestableNox.QueryScalar);

            // Assert
            mockCommand.Verify(x => x.ExecuteScalar(), Times.Once());
        }

        [Test]
        public void QueryScalarWithResultOfTypeInt_ReturnsInt()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteScalar()).Returns(1);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType)0))
               .Returns(mockCommand.Object);

            // Act
            int result = nox.ExecuteScalar<int>(TestableNox.QueryScalar);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
