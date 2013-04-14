using System;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

namespace Nox.Tests.NoxGenericRepositoryTests
{
    [TestFixture]
    public class Create
    {
        [Test]
        public void Entity_CallsNoxExecuteWithCorrectlyComposedInsertQuery()
        {
            // Arrange
            var noxGenericRepository = TestableNoxGenericRepository.Create();
            var fakeEntity = new TestEntity
            {
                TestEntityId = 123,
                TestPropertyDateTime = DateTime.Today,
                TestPropertyInt = 1,
                TestPropertyString = "TEST_STRING"
            };

            var expectedSqlQuery = "INSERT INTO TestEntity (TestEntityId, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestEntityId, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)";

            // Act
            noxGenericRepository.Create(fakeEntity);

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute(expectedSqlQuery, fakeEntity),
                                        Times.Once());
        }

        [Test]
        public void EntityWithPrimaryKeyButNoValueProvided_CallsNoxExecuteWithInsertQueryOmittingThePrimaryKeyColumn()
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
                                .Verify(x => x.Execute(expectedSqlQuery, fakeEntity),
                                        Times.Once());
        }

        [Test]
        public void EntityWithPrimaryKeyOfTypeGuidThatIsEmpty_CallsNoxExecuteWithInsertQueryOmittingTheGuidPrimaryKeyColumn()
        {
            // Arrange
            var nox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntityWithGuid>(nox.Object);
            var fakeEntity = new TestEntityWithGuid
            {
                TestPropertyInt = 0,
                TestPropertyString = "TEST_STRING"
            };

            var expectedSqlQuery = "INSERT INTO TestEntityWithGuid (TestPropertyString, TestPropertyInt) VALUES (@TestPropertyString, @TestPropertyInt)";

            // Act
            noxGenericRepository.Create(fakeEntity);

            // Assert
            nox.Verify(x => x.Execute(expectedSqlQuery, fakeEntity),
                       Times.Once());
        }

        [Test]
        public void EntityWithPrimaryKeyOfTypeGuidThatIsNotEmpty_CallsNoxExecuteWithInsertQueryIncludingTheGuidPrimaryKey()
        {
            // Arrange
            var nox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntityWithGuid>(nox.Object);
            var fakeEntity = new TestEntityWithGuid
            {
                TestEntityWithGuidId = Guid.NewGuid(),
                TestPropertyInt = 0,
                TestPropertyString = "TEST_STRING"
            };

            var expectedSqlQuery = "INSERT INTO TestEntityWithGuid (TestEntityWithGuidId, TestPropertyString, TestPropertyInt) VALUES (@TestEntityWithGuidId, @TestPropertyString, @TestPropertyInt)";

            // Act
            noxGenericRepository.Create(fakeEntity);

            // Assert
            nox.Verify(x => x.Execute(expectedSqlQuery, fakeEntity),
                       Times.Once());
        }

        [Test]
        public void EntityWithPrimaryKeyThatHasADifferentName_CallsNoxExecuteInsertQueryOmittingThePrimaryKey()
        {
            // Arrange
            var nox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntityWithDifferentIdColumnName>(nox.Object);
            var fakeEntity = new TestEntityWithDifferentIdColumnName
            {
                TestPropertyInt = 0,
                TestPropertyString = "TEST_STRING"
            };

            var expectedSqlQuery = "INSERT INTO TestEntityWithDifferentIdColumnName (TestPropertyString, TestPropertyInt) VALUES (@TestPropertyString, @TestPropertyInt)";

            // Act
            noxGenericRepository.Create(fakeEntity);

            // Assert
            nox.Verify(x => x.Execute(expectedSqlQuery, fakeEntity),
                       Times.Once());
        }

        [Test]
        public void EntityWithPrimaryKeyColumnNamedId_CallsNoxExecuteInsertQueryOmittingThePrimaryKey()
        {
            // Arrange
            var nox = new Mock<INox>();
            var noxGenericRepository = new NoxGenericRepository<TestEntityWithIdColumnName>(nox.Object);
            var fakeEntity = new TestEntityWithIdColumnName
            {
                TestPropertyInt = 0,
                TestPropertyString = "TEST_STRING"
            };

            var expectedSqlQuery = "INSERT INTO TestEntityWithIdColumnName (TestPropertyString, TestPropertyInt) VALUES (@TestPropertyString, @TestPropertyInt)";

            // Act
            noxGenericRepository.Create(fakeEntity);

            // Assert
            nox.Verify(x => x.Execute(expectedSqlQuery, fakeEntity),
                       Times.Once());
        }

        internal class TestEntityWithGuid
        {
            public Guid TestEntityWithGuidId { get; set; }
            public string TestPropertyString { get; set; }
            public int TestPropertyInt { get; set; }
        }

        internal class TestEntityWithDifferentIdColumnName
        {
            public int TestEntityWithDifferentIdColumnNameId { get; set; }
            public string TestPropertyString { get; set; }
            public int TestPropertyInt { get; set; }
        }

        internal class TestEntityWithIdColumnName
        {
            public int Id { get; set; }
            public string TestPropertyString { get; set; }
            public int TestPropertyInt { get; set; }
        }
    }
}
