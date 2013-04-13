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
                TestId = 123,
                TestPropertyDateTime = DateTime.Today,
                TestPropertyInt = 1,
                TestPropertyString = "TEST_STRING"
            };

            var expectedSqlQuery = "INSERT INTO TestEntity (TestId, TestPropertyString, TestPropertyInt, TestPropertyDateTime) VALUES (@TestId, @TestPropertyString, @TestPropertyInt, @TestPropertyDateTime)";

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
    }
}
