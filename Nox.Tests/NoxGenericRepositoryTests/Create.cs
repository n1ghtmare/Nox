using System;
using System.Collections.Generic;
using System.Reflection;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.NoxGenericRepositoryTests
{
    [TestFixture]
    public class Create
    {
        [Test, TestCaseSource("InsertTestCases")]
        public void Entity_CallsNoxExecuteWithCorrectlyComposedInsertQuery(object entity, string expectedQuery)
        {
            // Arrange
            Type genericClass = typeof (NoxGenericRepository<>);
            Type constructedClass = genericClass.MakeGenericType(entity.GetType());

            var mockNox = new Mock<INox>();
            var noxGenericRepository = Activator.CreateInstance(constructedClass, mockNox.Object);

            // Act
            MethodInfo method = constructedClass.GetMethod("Create");
            method.Invoke(noxGenericRepository, new[] { entity });

            // Assert
            mockNox.Verify(x => x.Execute(expectedQuery, entity),
                           Times.Once());
        }

        // TODO -> Using Create should autopopulate the primarykey property (if exists) with the SCOPE_IDENTITY()

        private static IEnumerable<TestCaseData> InsertTestCases
        {
            get
            {
                var testPropertyString   = "TEST_STRING";
                var testPropertyInt      = 1;
                var testPropertyDateTime = DateTime.Today;
                var testEntityId         = 123;
                var testEntityGuid       = Guid.NewGuid();

                yield return new TestCaseData(
                    new TestEntity1
                    {
                        TestEntity1Id        = testEntityId,
                        TestPropertyDateTime = testPropertyDateTime,
                        TestPropertyInt      = testPropertyInt,
                        TestPropertyString   = testPropertyString
                    },
                    "INSERT INTO TestEntity1 (TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestEntity1Id, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");

                yield return new TestCaseData(
                    new TestEntity1
                    {
                        TestPropertyDateTime = testPropertyDateTime,
                        TestPropertyInt      = testPropertyInt,
                        TestPropertyString   = testPropertyString
                    },
                    "INSERT INTO TestEntity1 (TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");

                yield return new TestCaseData(
                    new TestEntity2
                    {
                        TestPropertyDateTime = testPropertyDateTime,
                        TestPropertyInt      = testPropertyInt,
                        TestPropertyString   = testPropertyString
                    },
                    "INSERT INTO TestEntity2 (TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");
                
                yield return new TestCaseData(
                    new TestEntity2
                    {
                        TestEntity2Guid      = testEntityGuid,
                        TestPropertyDateTime = testPropertyDateTime,
                        TestPropertyInt      = testPropertyInt,
                        TestPropertyString   = testPropertyString
                    },
                    "INSERT INTO TestEntity2 (TestEntity2Guid, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestEntity2Guid, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");
                
                yield return new TestCaseData(
                    new TestEntity3
                    {
                        TestPropertyDateTime = testPropertyDateTime,
                        TestPropertyInt      = testPropertyInt,
                        TestPropertyString   = testPropertyString
                    },
                    "INSERT INTO TestEntity3 (TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");
                
                yield return new TestCaseData(
                    new TestEntity3
                    {
                        Id                   = testEntityId,
                        TestPropertyDateTime = testPropertyDateTime,
                        TestPropertyInt      = testPropertyInt,
                        TestPropertyString   = testPropertyString
                    },
                    "INSERT INTO TestEntity3 (Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@Id, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)");
            }
        }
        
    }
}
