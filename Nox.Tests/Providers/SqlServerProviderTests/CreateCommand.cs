using System.Data;
using System.Data.SqlClient;

using NUnit.Framework;

using Nox.Providers;

namespace Nox.Tests.Providers.SqlServerProviderTests
{
    [TestFixture]
    public class CreateCommand
    {
        private const string TestConnectionString = @"Data Source=test\SQLEXPRESS;Initial Catalog=sttikrdev;Integrated Security=True;";
        private const string SelectQuery = "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1";
        private const int CommandTimeout = 123;

        [Test]
        public void QuerySqlConnectionAndCommandType_ReturnsCorrectlyComposedSqlCommand()
        {
            // Arrange
            var sqlServerProvider = new SqlServerProvider(TestConnectionString) {CommandTimeout = CommandTimeout};
            var mockSqlConnection = new SqlConnection(TestConnectionString);
            
            // Act
            var command = sqlServerProvider.CreateCommand(SelectQuery, mockSqlConnection, CommandType.Text);
            
            // Assert
            Assert.IsInstanceOf<SqlCommand>(command);
            Assert.AreEqual(SelectQuery, command.CommandText);
            Assert.AreEqual(mockSqlConnection, command.Connection);
            Assert.AreEqual(CommandType.Text, command.CommandType);
            Assert.AreEqual(CommandTimeout, command.CommandTimeout);
        }
    }
}
