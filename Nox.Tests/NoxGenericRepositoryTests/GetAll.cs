using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            var mockNox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntity1>(mockNox.Object);
            var expectedSqlQuery = "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1";

            // Act
            noxGenericRepository.GetAll();

            // Assert
            mockNox.Verify(x => x.Execute<TestEntity1>(expectedSqlQuery),
                           Times.Once());
        }

        [Test]
        public void EntityWithDifferentNameAndProperties_CallsNoxExecuteWithCorrectlyComposedSelectQuery()
        {
            // Arrange
            var mockNox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntity2>(mockNox.Object);
            var expectedSqlQuery = "SELECT TestEntity2Guid, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity2";

            // Act
            noxGenericRepository.GetAll();

            // Assert
            mockNox.Verify(x => x.Execute<TestEntity2>(expectedSqlQuery),
                           Times.Once());
        }

        [Test]
        public void Entity_ReturnsCorrectResultsOfIEnumerableTEntity()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository.Create();

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