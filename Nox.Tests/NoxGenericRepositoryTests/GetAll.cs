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
            var noxGenericRepository = TestableNoxGenericRepository.Create();
            var expectedSqlQuery = "SELECT TestEntityId, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity";

            // Act
            noxGenericRepository.GetAll();

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute<TestEntity>(expectedSqlQuery),
                                        Times.Once());
        }

        [Test]
        public void Entity_ReturnsCorrectResultsOfIEnumerableTEntity()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository.Create();

            noxGenericRepository.MockNox
                                .Setup(x => x.Execute<TestEntity>(It.IsAny<string>()))
                                .Returns(new List<TestEntity> { new TestEntity(), new TestEntity() });

            // Act
            IEnumerable<TestEntity> results = noxGenericRepository.GetAll();

            // Assert
            Assert.AreEqual(2, results.Count());
        }
    }
}