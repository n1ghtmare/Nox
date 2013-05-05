using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.RepositoryTests
{
    [TestFixture]
    public class GetAll
    {
        [Test]
        public void Entity_CallsConductorExecuteWithCorrectlyComposedSelectQuery()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var selectQuery = "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1";

            repository.MockQueryComposer
                      .Setup(x => x.ComposeSelect(typeof (TestEntity1)))
                      .Returns(selectQuery);

            // Act
            repository.GetAll();

            // Assert
            repository.MockConductor
                      .Verify(x => x.Execute<TestEntity1>(selectQuery),
                              Times.Once());
        }

        [Test]
        public void Entity_CallsQueryComposerComposeSelect()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();

            // Act
            repository.GetAll();

            // Assert
            repository.MockQueryComposer
                      .Verify(x => x.ComposeSelect(typeof (TestEntity1)),
                              Times.Once());
        }

        [Test]
        public void Entity_ReturnsCorrectNumberOfResults()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();

            repository.MockConductor
                      .Setup(x => x.Execute<TestEntity1>(It.IsAny<string>()))
                      .Returns(new List<TestEntity1> {new TestEntity1(), new TestEntity1()});

            // Act
            IEnumerable<TestEntity1> results = repository.GetAll();

            // Assert
            Assert.AreEqual(2, results.Count());
        }
    }
}