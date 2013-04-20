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
        [Test, TestCaseSource("DeleteTestCases")]
        public void Entity_CallsNoxExecuteWithCorrectlyGeneratedDeleteQuery(object entity, string expectedQuery)
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

        [Test]
        public void EntityWithNoPrimaryKey_ThrowsAnException()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity4>.Create();
            var fakeEntity = new TestEntity4();
            
            // Act
            var exception = Assert.Throws<Exception>(() => noxGenericRepository.Delete(fakeEntity));

            // Assert
            Assert.AreEqual(exception.Message, "Can't compose a delete query - unable to detect primary key");
        }

        private static IEnumerable<TestCaseData> DeleteTestCases
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
                    "DELETE FROM TestEntity1 WHERE TestEntity1Id = @TestEntity1Id");

                yield return new TestCaseData(
                    new TestEntity2
                    {
                        TestEntity2Guid    = testEntityGuid,
                        TestPropertyInt    = testPropertyInt,
                        TestPropertyString = testPropertyString
                    },
                    "DELETE FROM TestEntity2 WHERE TestEntity2Guid = @TestEntity2Guid");

                yield return new TestCaseData(
                    new TestEntity3
                    {
                        Id                  = testEntityId,
                        TestPropertyInt     = testPropertyInt,
                        TestPropertyString  = testPropertyString
                    },
                    "DELETE FROM TestEntity3 WHERE Id = @Id");
            }
        }
    }
}
