using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.NoxGenericRepositoryTests
{
    [TestFixture]
    public class GetAll
    {
        [Test]
        public void Entity_CallsNoxExecuteWithCorrectlyComposedSelectQuery()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity1>.Create();
            var expectedSqlQuery = "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1";

            // Act
            noxGenericRepository.GetAll();

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute<TestEntity1>(expectedSqlQuery),
                                        Times.Once());
        }

        [Test]
        public void EntityWithDifferentNameAndProperties_CallsNoxExecuteWithCorrectlyComposedSelectQuery()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity2>.Create();
            var expectedSqlQuery = "SELECT TestEntity2Guid, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity2";

            // Act
            noxGenericRepository.GetAll();

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute<TestEntity2>(expectedSqlQuery),
                                        Times.Once());
        }

        [Test]
        public void Entity_ReturnsCorrectNumberOfResults()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity1>.Create();

            noxGenericRepository.MockNox
                                .Setup(x => x.Execute<TestEntity1>(It.IsAny<string>()))
                                .Returns(new List<TestEntity1> { new TestEntity1(), new TestEntity1() });

            // Act
            IEnumerable<TestEntity1> results = noxGenericRepository.GetAll();

            // Assert
            Assert.AreEqual(2, results.Count());
        }

    }
}