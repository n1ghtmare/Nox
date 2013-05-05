using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.RepositoryTests
{
    [TestFixture]
    public class Create
    {
        [Test]
        public void Entity_CallsQueryComposerComposeInsert()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1
            {
                TestEntity1Id = 123,
            };

            // Act
            repository.Create(entity);

            // Assert
            repository.MockQueryComposer
                      .Verify(x => x.ComposeInsert(entity.GetType()),
                              Times.Once());
        }

        [Test]
        public void EntityWithNoPrimaryKeyValue_CallsQueryComposerComposeInsertWithPropertiesExcludingThePrimaryKey()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1();

            // Act
            repository.Create(entity);

            // Assert
            repository.MockQueryComposer
                      .Verify(x => x.ComposeInsert(entity.GetType(), "TestEntity1Id"),
                              Times.Once());
        }

        [Test]
        public void Entity_CallsConductorExecuteScalarWithCorrectlyComposedInserQuery()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1();
            var insertQuery = "INSERT INTO TestEntity1 (TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestEntity1Id, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)";
            
            repository.MockQueryComposer
                      .Setup(x => x.ComposeInsert(entity.GetType(), It.IsAny<string>()))
                      .Returns(insertQuery);
            // Act
            repository.Create(entity);

            // Assert
            repository.MockConductor
                      .Verify(x => x.ExecuteScalar<object>(insertQuery, entity),
                              Times.Once());
        }

        [Test]
        public void Entity_AfterInsertReturnsEntityWithThePrimaryKeyPropertyContainingIdentityValue()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1();

            repository.MockConductor
                      .Setup(x => x.ExecuteScalar<object>(It.IsAny<string>(), It.IsAny<object>()))
                      .Returns(123);

            // Act
            repository.Create(entity);

            // Assert
            Assert.AreEqual(123, entity.TestEntity1Id);
        }
    }
}
