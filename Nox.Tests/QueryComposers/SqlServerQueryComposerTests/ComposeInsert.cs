using System.Collections.Generic;

using NUnit.Framework;

using Nox.QueryComposers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.QueryComposers.SqlServerQueryComposerTests
{
    [TestFixture]
    public class ComposeInsert
    {
        [Test, TestCaseSource("InsertTestCases")]
        public void EntityType_ReturnsCorrectlyComposedInsertQuery(object entityType, string expectedQuery)
        {
            // Arrange
            var sqlServerQueryComposer = new SqlServerQueryComposer();

            // Act
            var insertQuery = sqlServerQueryComposer.ComposeInsert(entityType.GetType());

            // Assert
            Assert.AreEqual(expectedQuery, insertQuery);
        }

        [Test, TestCaseSource("InsertTestCasesWithPrimaryKeyName")]
        public void EntityTypeAndSelectedProperties_ReturnsCorrectlyComposedInsertQueryContainingAllPropertiesExcludingThePrimaryKey(object entityType, string primaryKeyName, string expectedQuery)
        {
            // Arrange
            var sqlServerQueryComposer = new SqlServerQueryComposer();

            // Act
            var insertQuery = sqlServerQueryComposer.ComposeInsert(entityType.GetType(), primaryKeyName);

            // Assert
            Assert.AreEqual(expectedQuery, insertQuery);
        }

        private static IEnumerable<TestCaseData> InsertTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new TestEntity1(),
                    "INSERT INTO TestEntity1 (TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestEntity1Id, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");

                yield return new TestCaseData(
                    new TestEntity2(),
                    "INSERT INTO TestEntity2 (TestEntity2Guid, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestEntity2Guid, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");

                yield return new TestCaseData(
                    new TestEntity3(),
                    "INSERT INTO TestEntity3 (Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@Id, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");
            }
        }

        private static IEnumerable<TestCaseData> InsertTestCasesWithPrimaryKeyName
        {
            get
            {
                yield return new TestCaseData(
                    new TestEntity1(),
                    "TestEntity1Id",
                    "INSERT INTO TestEntity1 (TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestPropertyString, @TestPropertyInt, @TestPropertyDateTime) SELECT SCOPE_IDENTITY()");

                yield return new TestCaseData(
                    new TestEntity2(),
                    "TestEntity2Guid",
                    "INSERT INTO TestEntity2 (TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestPropertyString, @TestPropertyInt, @TestPropertyDateTime) SELECT SCOPE_IDENTITY()");

                yield return new TestCaseData(
                    new TestEntity3(),
                    "Id",
                    "INSERT INTO TestEntity3 (TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestPropertyString, @TestPropertyInt, @TestPropertyDateTime) SELECT SCOPE_IDENTITY()");
            }
        }
    }
}