using System.Collections.Generic;

using NUnit.Framework;

using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.QueryComposerTests.SqlServerQueryComposerTests
{
    [TestFixture]
    public class ComposeDelete
    {
        [Test, TestCaseSource("DeleteTestCases")]
        public void EntityType_ReturnsCorrectlyComposedInsertQuery(object entityType, string primaryKeyName, string expectedQuery)
        {
            // Arrange
            var sqlServerQueryComposer = new SqlServerQueryComposer();

            // Act
            var deleteQuery = sqlServerQueryComposer.ComposeDelete(entityType.GetType(), primaryKeyName);

            // Assert
            Assert.AreEqual(expectedQuery, deleteQuery);
        }

        private static IEnumerable<TestCaseData> DeleteTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new TestEntity1(),
                    "TestEntity1Id",
                    "DELETE FROM TestEntity1 WHERE TestEntity1Id = @TestEntity1Id");

                yield return new TestCaseData(
                    new TestEntity2(),
                    "TestEntity2Guid",
                    "DELETE FROM TestEntity2 WHERE TestEntity2Guid = @TestEntity2Guid");

                yield return new TestCaseData(
                    new TestEntity3(),
                    "Id",
                    "DELETE FROM TestEntity3 WHERE Id = @Id");
            }
        }
    }
}
