using System.Data.OleDb;

using NUnit.Framework;

using Nox.Providers;

namespace Nox.Tests.Providers.OleDbProviderTests
{
    [TestFixture]
    public class CreateConnection
    {
        private const string TestConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\test\test.xlsx;Extended Properties=""Excel 12.0 Xml;HDR=YES"";";

        [Test]
        public void ConnectionString_ReturnsOleDbConnection()
        {
            // Arrange
            var oleDbProvider = new OleDbProvider(TestConnectionString);

            // Act
            var oleDbConnection = oleDbProvider.CreateConnection();

            // Assert
            Assert.IsInstanceOf<OleDbConnection>(oleDbConnection);
            Assert.AreEqual(TestConnectionString, oleDbConnection.ConnectionString);
        }
    }
}
