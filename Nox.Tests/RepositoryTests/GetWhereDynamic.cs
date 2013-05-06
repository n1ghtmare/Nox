using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

using Moq;
using NUnit.Framework;
using Nox.Providers;
using Nox.Repositories;
using Nox.Tests.Helpers;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.RepositoryTests
{
    [TestFixture]
    public class GetWhereDynamic
    {
        [Test]
        public void UnderscoreWithOneSegment_CallsQueryComposerComposeSelectWithTheInvokingMethodName()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();

            // Act
            ((dynamic)repository).GetWhere_TestPropertyString("TEST_STRING");

            // Assert
            repository.MockQueryComposer
                      .Verify(x => x.ComposeSelect(typeof(TestEntity1), "GetWhere_TestPropertyString"),
                              Times.Once());
        }

        [Test]
        public void FirstSegmentDoesntBeginWithGetWhere_ThrowAnException()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();

            // Act
            // Assert
            Assert.Throws<RuntimeBinderException>(() => ((dynamic) repository).NotExisting());
        }

        [Test]
        public void FirstSegmentOnlyWithNoOthers_ThrowsAnException()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();

            // Act
            Exception ex = Assert.Throws<Exception>(() => ((dynamic)repository).GetWhere_());

            // Assert
            Assert.AreEqual("Can't detect parameter segments in your invokation", ex.Message);
        }

        [Test]
        public void UnderscoreWithTwoSegmentsWithAndKeyword_CallsConductorWithCorrectQueryAndParameters()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var selectQuery =
                "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1 WHERE TestPropertyString = @TestPropertyString AND TestPropertyInt = @TestPropertyInt";
            var parameters = new Dictionary<string, object>
            {
                {"TestPropertyString", "TEST_STRING"},
                {"TestPropertyInt", 123}
            };

            repository.MockQueryComposer
                      .Setup(
                          x => x.ComposeSelect(typeof (TestEntity1), "GetWhere_TestPropertyString_And_TestPropertyInt"))
                      .Returns(selectQuery);

            // Act
            ((dynamic) repository).GetWhere_TestPropertyString_And_TestPropertyInt("TEST_STRING", 123);

            // Assert
            repository.MockConductor
                      .Verify(x => x.Execute<TestEntity1>(selectQuery, parameters),
                              Times.Once());
        }

        [Test]
        public void UnderscoreWithTwoSegmentsWithOrKeyword_CallsConductorWithCorrectQueryAndParameters()
        {
            // Arrange
            var repository = TestableRepository<TestEntity1>.Create();
            var selectQuery =
                "SELECT TestEntity1Id, TestPropertyString, TestPropertyInt, TestPropertyDateTime FROM TestEntity1 WHERE TestPropertyString = @TestPropertyString AND TestPropertyInt = @TestPropertyInt";
            var parameters = new Dictionary<string, object>
            {
                {"TestPropertyString", "TEST_STRING"},
                {"TestPropertyInt", 123}
            };

            repository.MockQueryComposer
                      .Setup(
                          x => x.ComposeSelect(typeof(TestEntity1), "GetWhere_TestPropertyString_Or_TestPropertyInt"))
                      .Returns(selectQuery);

            // Act
            ((dynamic)repository).GetWhere_TestPropertyString_Or_TestPropertyInt("TEST_STRING", 123);

            // Assert
            repository.MockConductor
                      .Verify(x => x.Execute<TestEntity1>(selectQuery, parameters),
                              Times.Once());
        }

        [Test]
        public void MainSegmentDifferentLowerUpperCase_CallsQueryComposerComposeSelectWithTheCorrectType()
        {
            // Arrange     
            var repository = TestableRepository<TestEntity1>.Create();

            // Act
            ((dynamic) repository).getWhere_TestPropertyString("TEST_STRING");

            // Assert
            repository.MockQueryComposer
                       .Verify(x => x.ComposeSelect(typeof(TestEntity1), "getWhere_TestPropertyString"),
                               Times.Once());
        }
    }

    internal class Accounts
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
    }
}
