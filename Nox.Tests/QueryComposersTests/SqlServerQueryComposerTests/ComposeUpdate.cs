using System.Collections.Generic;

using NUnit.Framework;

using Nox.QueryComposers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.QueryComposersTests.SqlServerQueryComposerTests
{
    [TestFixture]
    public class ComposeUpdate
    {
        [Test, TestCaseSource("UpdateTestCases")]
        public void EntityType_ReturnsCorrectlyComposedInsertQuery(object entityType, string primaryKeyName, string expectedQuery)
        {
            // Arrange
            var sqlServerQueryComposer = new SqlServerQueryComposer();

            // Act
            var updateQuery = sqlServerQueryComposer.ComposeUpdate(entityType.GetType(), primaryKeyName);

            // Assert
            Assert.AreEqual(expectedQuery, updateQuery);
        }

        private static IEnumerable<TestCaseData> UpdateTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new TestEntity1(),
                    "TestEntity1Id",
                    "UPDATE TestEntity1 SET TestPropertyString = @TestPropertyString, TestPropertyInt = @TestPropertyInt, TestPropertyDateTime = @TestPropertyDateTime WHERE TestEntity1Id = @TestEntity1Id");

                yield return new TestCaseData(
                    new TestEntity2(),
                    "TestEntity2Guid",
                    "UPDATE TestEntity2 SET TestPropertyString = @TestPropertyString, TestPropertyInt = @TestPropertyInt, TestPropertyDateTime = @TestPropertyDateTime WHERE TestEntity2Guid = @TestEntity2Guid");

                yield return new TestCaseData(
                    new TestEntity3(),
                    "Id",
                    "UPDATE TestEntity3 SET TestPropertyString = @TestPropertyString, TestPropertyInt = @TestPropertyInt, TestPropertyDateTime = @TestPropertyDateTime WHERE Id = @Id");
            }
        }
    }
}
