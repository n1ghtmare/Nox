using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

namespace Nox.Tests.ConductorTests
{
    [TestFixture]
    public class ExecuteScalar
    {
        [Test]
        public void QueryScalar_CallsProviderCreateConnection()
        {
            // Arrange
            var conductor = TestableConductor.Create();

            // Act
            conductor.ExecuteScalar<int>(TestableConductor.QueryScalar);

            // Assert
            conductor.MockProvider
                     .Verify(x => x.CreateConnection(), Times.Once());
        }

        [Test]
        public void QueryScalar_CallsProviderCreateCommand()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockConnection = new Mock<IDbConnection>();

            conductor.MockProvider
                     .Setup(x => x.CreateConnection())
                     .Returns(mockConnection.Object);

            // Act
            conductor.ExecuteScalar<int>(TestableConductor.QueryScalar);

            // Assert
            conductor.MockProvider
                     .Verify(
                         x => x.CreateCommand(TestableConductor.QueryScalar, mockConnection.Object, (CommandType) 0),
                         Times.Once());
        }

        [Test]
        public void QueryScalarWithParameters_CallsProviderCreateParameters()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var parameters = new { TestPropertyString = "TEST_STRING", TestPropertyInt = 123 };

            // Act
            conductor.ExecuteScalar<int>(TestableConductor.QueryScalarWithParameters, parameters);

            // Assert
            conductor.MockProvider
                     .Verify(x => x.CreateParameters(
                         It.Is<Dictionary<string, object>>(
                             d => (string)  d["TestPropertyString"] == "TEST_STRING" &&
                                  (int)     d["TestPropertyInt"] == 123)),
                             Times.Once());
        }

        [Test]
        public void QueryScalarWithParameters_CallsProviderCommandAddParameterForEachProvidedParameter()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var parameters = new { TestPropertyString = "TEST_STRING", TestPropertyInt = 123 };
            var mockCommand = new Mock<IDbCommand>();
            var mockParameterCollection = new Mock<IDataParameterCollection>();

            mockCommand.Setup(x => x.Parameters).Returns(mockParameterCollection.Object);
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(0);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            conductor.MockProvider
                     .Setup(x => x.CreateParameters(It.IsAny<IDictionary<string, object>>()))
                     .Returns(new List<IDbDataParameter>
                     {
                         new SqlParameter("@TestFirstParameter", "FirstParameterValue"),
                         new SqlParameter("@TestSecondParameter", "SecondParameterValue")
                     });

            // Act
            conductor.ExecuteScalar<int>(TestableConductor.QueryScalarWithParameters, parameters);

            // Assert
            mockParameterCollection
                .Verify(x => x.Add(It.IsAny<IDataParameter>()),
                        Times.Exactly(2));
        }

        [Test]
        public void QueryScalarWithParameters_CallsProviderConnectionOpen()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockConnection = new Mock<IDbConnection>();

            conductor.MockProvider.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            // Act
            conductor.ExecuteScalar<int>(TestableConductor.QueryScalar);

            // Assert
            mockConnection.Verify(x => x.Open(), Times.Once());
        }

        [Test]
        public void QueryScalar_CallsProviderCommandExecuteScalar()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteScalar()).Returns(0);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            // Act
            conductor.ExecuteScalar<int>(TestableConductor.QueryScalar);

            // Assert
            mockCommand.Verify(x => x.ExecuteScalar(), Times.Once());
        }

        [Test]
        public void QueryScalarWithResultOfTypeInt_ReturnsInt()
        {
            // Arrange
            var conductor = TestableConductor.Create();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteScalar()).Returns(1);

            conductor.MockProvider
                     .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), (CommandType) 0))
                     .Returns(mockCommand.Object);

            // Act
            int result = conductor.ExecuteScalar<int>(TestableConductor.QueryScalar);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
