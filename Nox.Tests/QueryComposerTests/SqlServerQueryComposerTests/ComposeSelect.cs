using System.Collections.Generic;

using NUnit.Framework;

using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.QueryComposerTests.SqlServerQueryComposerTests
{
    [TestFixture]
    public class ComposeSelect
    {
        [Test, TestCaseSource("SelectTestCases")]
        public void EntityType_ReturnsCorrectlyComposedInsertQuery(object entityType, string expectedQuery)
        {
            // Arrange
            var sqlServerQueryComposer = new SqlServerQueryComposer();

            // Act
            var selectQuery = sqlServerQueryComposer.ComposeSelect(entityType.GetType());

            // Assert
            Assert.AreEqual(expectedQuery, selectQuery);
        }

        private static IEnumerable<TestCaseData> SelectTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new TestEntity1(),
                    "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1");

                yield return new TestCaseData(
                    new TestEntity2(),
                    "SELECT TestEntity2Guid, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity2");

                yield return new TestCaseData(
                    new TestEntity3(),
                    "SELECT Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity3");
            }
        }
    }
}
