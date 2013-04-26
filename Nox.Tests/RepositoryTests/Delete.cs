using System;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.RepositoryTests
{
    [TestFixture]
    public class Delete
    {
        [Test]
        public void EntityWithNoPrimaryKey_ThrowsAnException()
        {
            // Arrange
            var noxGenericRepository = TestableRepository<TestEntity4>.Create();
            var fakeEntity = new TestEntity4();
            
            // Act
            var exception = Assert.Throws<Exception>(() => noxGenericRepository.Delete(fakeEntity));

            // Assert
            Assert.AreEqual(exception.Message, "Can't compose a delete query - unable to detect primary key");
        }

        [Test]
        public void Entity_CallsQueryComposerComposeDelete()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1();

            // Act
            repository.Delete(entity);

            // Assert
            repository.MockQueryComposer
                      .Verify(x => x.ComposeDelete(entity.GetType(), "TestEntity1Id"),
                              Times.Once());
        }

        [Test]
        public void Entity_CallsNoxExecuteWithCorrectlyComposedDeleteQuery()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var entity = new TestEntity1();
            var deleteQuery = "DELETE FROM TestEntity1 WHERE TestEntity1Id = @TestEntity1Id";

            repository.MockQueryComposer
                      .Setup(x => x.ComposeDelete(entity.GetType(), "TestEntity1Id"))
                      .Returns(deleteQuery);

            // Act
            repository.Delete(entity);

            // Assert
            repository.MockNox
                      .Verify(x => x.Execute(deleteQuery, entity),
                              Times.Once());
        }

    }
}
