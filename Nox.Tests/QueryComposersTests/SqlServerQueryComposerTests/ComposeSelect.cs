using System.Collections.Generic;
using NUnit.Framework;

using Nox.QueryComposers;

using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.QueryComposersTests.SqlServerQueryComposerTests
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

        [Test, TestCaseSource("SelectTestCasesWithInvokingMethodName")]
        public void EntityTypeWithInvokingMethodName_ReturnsCorrectlyComposedInsertQuery(object entityType, string invokingMethodName, string expectedQuery)
        {
            // Arrange
            var sqlServerQueryComposer = new SqlServerQueryComposer();

            // Act
            var selectQuery = sqlServerQueryComposer.ComposeSelect(entityType.GetType(), invokingMethodName);

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

        private static IEnumerable<TestCaseData> SelectTestCasesWithInvokingMethodName
        {
            get
            {
                yield return new TestCaseData(
                    new TestEntity1(),
                    "GetWhere_TestPropertyString_And_TestPropertyInt",
                    "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1 WHERE TestPropertyString = @TestPropertyString AND TestPropertyInt = @TestPropertyInt");

                yield return new TestCaseData(
                    new TestEntity2(),
                    "GetWhere_TestPropertyString_Or_TestPropertyInt",
                    "SELECT TestEntity2Guid, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity2 WHERE TestPropertyString = @TestPropertyString OR TestPropertyInt = @TestPropertyInt");

                yield return new TestCaseData(
                    new TestEntity3(),
                    "GetWhere_Id",
                    "SELECT Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity3 WHERE Id = @Id");
            }
        }
    }
}
