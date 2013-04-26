using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.RepositoryTests
{
    [TestFixture]
    public class Get
    {
        [Test]
        public void EntityWhereAndParameters_CallsNoxExecuteWithCorrectlyComposedQuery()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var selectQueryBase = "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1";
            var where = "TestPropertyInt = @TestPropertyInt";
            var expectedQuery = string.Format("{0} WHERE {1}", selectQueryBase, where);
            var parameters = new {TestPropertyInt = 123};

            repository.MockQueryComposer
                      .Setup(x => x.ComposeSelect(typeof (TestEntity1)))
                      .Returns(selectQueryBase);

            // Act
            repository.Get(where, parameters);

            // Assert
            repository.MockNox
                      .Verify(x => x.Execute<TestEntity1>(expectedQuery, parameters),
                              Times.Once());
        }

        [Test]
        public void EntityWhereAndParameters_CallsQueryComposerComposeSelect()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var where = "TestPropertyInt = @TestPropertyInt";

            // Act
            repository.Get(where, new {TestPropertyInt = 123});

            // Assert
            repository.MockQueryComposer
                      .Verify(x => x.ComposeSelect(typeof(TestEntity1)),
                              Times.Once());
        }

        [Test]
        public void EntityWhereAndParameters_ReturnsCorrectNumberOfResults()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();

            repository.MockNox
                      .Setup(x => x.Execute<TestEntity1>(It.IsAny<string>(), It.IsAny<object>()))
                      .Returns(new List<TestEntity1> {new TestEntity1(), new TestEntity1()});
            
            // Act
            IEnumerable<TestEntity1> results = repository.Get("TestPropertyInt = @TestPropertyInt", new { TestPropertyInt = 123 });

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [Test]
        public void NullParameters_ThrowsAnException()
        {
            // Arrange
            var noxGenericRepository = TestableRepository<TestEntity1>.Create();

            // Act
            var exception =
                Assert.Throws<ArgumentNullException>(
                    () => noxGenericRepository.Get("TestPropertyInt = @TestPropertyInt", null));

            // Assert
            Assert.AreEqual("Can't pass null parameters, make sure you pass valid query parameters\r\nParameter name: parameters", exception.Message);
        }
    }
}
