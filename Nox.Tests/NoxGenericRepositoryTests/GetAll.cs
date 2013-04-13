using System.Collections.Generic;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

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
            var expectedSqlQuery = "SELECT TestId, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity";

            // Act
            IEnumerable<TestEntity> results = noxGenericRepository.GetAll();

            // Assert
            noxGenericRepository.MockNox
                                .Verify(x => x.Execute<TestEntity>(expectedSqlQuery),
                                        Times.Once());
        }


    }
}