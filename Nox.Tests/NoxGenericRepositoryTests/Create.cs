using System;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

namespace Nox.Tests.NoxGenericRepositoryTests
{
    [TestFixture]
    public class Create
    {
        [Test]
        public void Entity_CallsNoxExecuteWithCorrectlyBuildQuery()
        {
            // Arrange
            var mockNox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntity>(mockNox.Object);
            var fakeEntity = new TestEntity
            {
                TestPropertyDateTime = DateTime.Today,
                TestPropertyInt = 1,
                TestPropertyString = "TEST_STRING"
            };

            var expectedSqlQuery = "INSERT INTO TestEntity (TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)";

            // Act
            noxGenericRepository.Create(fakeEntity);

            // Assert
            mockNox.Verify(
                x => x.Execute(expectedSqlQuery, It.IsAny<TestEntity>()),
                          Times.Exactly(1));
        }

        [Test]
        public void EntityWithNoProperties_ThrowsAnException()
        {
            // Arrange
            var mockNox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntityWithNoProperties>(mockNox.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => noxGenericRepository.Create(new TestEntityWithNoProperties()));

            // Assert
            Assert.AreEqual("Insert parameters can't be empty, make sure your entity has properties",
                            exception.Message);
        }

        internal class TestEntityWithNoProperties { }
    }

    

}
