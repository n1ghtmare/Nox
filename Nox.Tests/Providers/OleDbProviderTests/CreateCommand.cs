using System.Data;
using System.Data.OleDb;

using NUnit.Framework;

using Nox.Providers;

namespace Nox.Tests.Providers.OleDbProviderTests
{
    [TestFixture]
    public class CreateCommand
    {
        private const string TestConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\test\test.xlsx;Extended Properties=""Excel 12.0 Xml;HDR=YES"";";
        private const string SelectQuery = "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1";
        private const int CommandTimeout = 123;

        [Test]
        public void QueryOleDbConnectionAndCommandType_ReturnsCorrectlyComposedOleDbCommand()
        {
            // Arrange
            var oleDbProvider = new OleDbProvider(TestConnectionString) { CommandTimeout = CommandTimeout };
            var mockOleDbConnection = new OleDbConnection(TestConnectionString);

            // Act
            var command = oleDbProvider.CreateCommand(SelectQuery, mockOleDbConnection, CommandType.Text);
            
            // Assert
            Assert.IsInstanceOf<OleDbCommand>(command);
            Assert.AreEqual(SelectQuery, command.CommandText);
            Assert.AreEqual(mockOleDbConnection, command.Connection);
            Assert.AreEqual(CommandType.Text, command.CommandType);
            Assert.AreEqual(CommandTimeout, command.CommandTimeout);
        }
    }
}
