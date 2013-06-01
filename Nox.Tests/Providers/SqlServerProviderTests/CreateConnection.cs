using System.Data.SqlClient;

using NUnit.Framework;

using Nox.Providers;

namespace Nox.Tests.Providers.SqlServerProviderTests
{
    [TestFixture]
    public class CreateConnection
    {
        const string TestConnectionString = @"Data Source=test\SQLEXPRESS;Initial Catalog=sttikrdev;Integrated Security=True;";

        [Test]
        public void ConnectionString_ReturnsSqlConnection()
        {
            // Arrange
            var sqlServerProvider = new SqlServerProvider(TestConnectionString);

            // Act
            var sqlConnection = sqlServerProvider.CreateConnection();

            // Assert
            Assert.IsInstanceOf<SqlConnection>(sqlConnection);
            Assert.AreEqual(TestConnectionString, sqlConnection.ConnectionString);
        }
    }
}
