using System;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

namespace Nox.Tests.NoxGenericRepositoryTests
{
    [TestFixture]
    public class Constructor
    {
        [Test]
        public void EntityWithNoProperties_ThrowsAnException()
        {
            // Arrange
            var mockNox = new Mock<INox>();

            // Act
            var exception = Assert.Throws<Exception>(() => new NoxGenericRepository<TestEntityWithNoProperties>(mockNox.Object));

            // Assert
            Assert.AreEqual("Insert parameters can't be empty, make sure your entity has properties",
                            exception.Message);
        }

        internal class TestEntityWithNoProperties { }
    }

    [TestFixture]
    public class Create
    {
        [Test]
        public void Entity_CallsNoxExecuteWithCorrectlyBuildInsertQuery()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository.Create();
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
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute(expectedSqlQuery, It.IsAny<TestEntity>()),
                                        Times.Once());
        }
    }

    [TestFixture]
    public class GetAll
    {
        [Test]
        public void Entity_CallsNoxExecuteWithCorrectlyBuildSelectQuery()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository.Create();
            var expectedSqlQuery = "SELECT TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity";

            // Act
            IEnumerable<TestEntity> results = noxGenericRepository.GetAll();

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute<TestEntity>(expectedSqlQuery),
                                        Times.Once());
        }
    }
}
