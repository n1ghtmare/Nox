using System;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.RepositoryTests
{
    [TestFixture]
    public class Update
    {
        [Test]
        public void Entity_CallsQueryComposerComposeUpdate()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1();

            // Act
            repository.Update(entity);

            // Assert
            repository.MockQueryComposer
                      .Verify(x => x.ComposeUpdate(entity.GetType(), "TestEntity1Id"),
                              Times.Once());
        }

        [Test]
        public void Entity_CallsNoxExecuteWithCorrectlyComposedUpdateQuery()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1();
            var updateQuery = "UPDATE TestEntity1 SET TestPropertyString = @TestPropertyString, TestPropertyInt = @TestPropertyInt, TestPropertyDateTime = @TestPropertyDateTime WHERE TestEntity1Id = @TestEntity1Id";

            repository.MockQueryComposer
                      .Setup(x => x.ComposeUpdate(entity.GetType(), "TestEntity1Id"))
                      .Returns(updateQuery);

            // Act
            repository.Update(entity);

            // Assert
            repository.MockNox
                      .Verify(x => x.Execute(updateQuery, entity),
                              Times.Once());
        }

        [Test]
        public void EntityWithNoPrimaryKey_ThrowsAnException()
        {
            // Arrange
            var noxGenericRepository = TestableRepository<TestEntity4>.Create();
            var fakeEntity = new TestEntity4();

            // Act
            var exception = Assert.Throws<Exception>(() => noxGenericRepository.Update(fakeEntity));

            // Assert
            Assert.AreEqual(exception.Message, "Can't compose an update query - unable to detect primary key");
        }
    }
}
