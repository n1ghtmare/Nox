using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.NoxGenericRepositoryTests
{
    [TestFixture]
    public class Delete
    {
        [Test]
        public void EntityAPrimaryKey_CallsNoxExecuteWithCorrectlyGeneratedSqlQuery()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository.Create();
            var fakeEntity = new TestEntity
            {
                TestEntityId = 123,
                TestPropertyDateTime = DateTime.Today,
                TestPropertyInt = 1,
                TestPropertyString = "TEST_STRING"
            };
            var expectedSqlQuery = "DELETE FROM TestEntity WHERE TestEntityId = @TestEntityId";

            // Act
            noxGenericRepository.Delete(fakeEntity);

            // Assert
            noxGenericRepository.MockNox
                .Verify(x => x.Execute(expectedSqlQuery, fakeEntity), 
                Times.Once());
        }

        [Test, TestCaseSource("DeleteTestCases")]
        public void Entity_CallsNoxExecuteWithCorrectlyGeneratedSqlQuery(object entity, string expectedQuery)
        {
            // Arrange
            Type genericClass = typeof (NoxGenericRepository<>);
            Type constructedClass = genericClass.MakeGenericType(entity.GetType());

            var mockNox = new Mock<INox>();
            var noxGenericRepository = Activator.CreateInstance(constructedClass, mockNox.Object);

            // Act
            MethodInfo method = constructedClass.GetMethod("Delete");
            method.Invoke(noxGenericRepository, new[] { entity });


            // Assert
            mockNox.Verify(x => x.Execute(expectedQuery, entity),
                Times.Once());
        }

        private static IEnumerable<TestCaseData> DeleteTestCases
        {
            get
            {
                string testPropertyString     = "TEST_STRING";
                int testPropertyInt           = 1;
                DateTime testPropertyDateTime = DateTime.Today;
                int testEntityId              = 123;

                yield return new TestCaseData(
                    new TestEntity
                    {
                        TestEntityId         = testEntityId,
                        TestPropertyDateTime = testPropertyDateTime,
                        TestPropertyInt      = testPropertyInt,
                        TestPropertyString   = testPropertyString
                    }, 
                    "DELETE FROM TestEntity WHERE TestEntityId = @TestEntityId");

                yield return new TestCaseData(
                    new TestEntityWithDifferentIdColumnName
                    {
                        TestEntityWithDifferentIdColumnNameId = testEntityId,
                        TestPropertyInt = testPropertyInt,
                        TestPropertyString = testPropertyString
                    },
                    "DELETE FROM TestEntityWithDifferentIdColumnName WHERE TestEntityWithDifferentIdColumnNameId = @TestEntityWithDifferentIdColumnNameId");
            }
        }
    }
}
