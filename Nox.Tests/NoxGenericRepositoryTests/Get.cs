using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.NoxGenericRepositoryTests
{
    [TestFixture]
    public class Get
    {
        [TestCase("TestPropertyInt = @TestPropertyInt")]
        [TestCase("TestPropertyInt = @TestPropertyInt AND TestPropertyString = @TestPropertyString")]
        [TestCase("TestPropertyString = @TestPropertyString")]
        public void EntityWhereAndParameters_CallsNoxExecuteAndAppendsWhereToTheCorrectlySelectBase(string where)
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity1>.Create();
            var expectedQuery = string.Format("SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1 WHERE {0}", where);
            var parameters = new { TestPropertyInt = 1 };
            // Act
            noxGenericRepository.Get(where, parameters);

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute<TestEntity1>(expectedQuery, parameters),
                                        Times.Once());
        }

        [Test]
        public void EntityWithDifferentNameAndPropertiesAndWhere_CallsNoxExecuteWithCorrectlyComposedSelect()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity2>.Create();
            var expectedQuery = "SELECT TestEntity2Guid, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity2 WHERE TestEntity2Guid = @TestEntity2Guid";
            var parameters = new {TestEntity2Guid = Guid.NewGuid()};

            // Act
            noxGenericRepository.Get("TestEntity2Guid = @TestEntity2Guid", parameters);

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute<TestEntity2>(expectedQuery, parameters),
                                        Times.Once());
        }

        [Test]
        public void EntityWhereAndParameters_ReturnsCorrectNumberOfResults()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity1>.Create();
            noxGenericRepository.MockNox
                                .Setup(x => x.Execute<TestEntity1>(It.IsAny<string>(), It.IsAny<object>()))
                                .Returns(new List<TestEntity1> {new TestEntity1(), new TestEntity1()});
            
            // Act
            IEnumerable<TestEntity1> results = noxGenericRepository.Get("TestPropertyInt = @TestPropertyInt", new { TestPropertyInt = 123 });

            // Assert
            Assert.AreEqual(2, results.Count());
        }

        [Test]
        public void NullParameters_ThrowsAnException()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository<TestEntity1>.Create();

            // Act
            var exception =
                Assert.Throws<ArgumentNullException>(
                    () => noxGenericRepository.Get("TestPropertyInt = @TestPropertyInt", null));

            // Assert
            Assert.AreEqual("Can't pass null parameters, make sure you pass valid query parameters\r\nParameter name: parameters", exception.Message);
        }
    }
}
