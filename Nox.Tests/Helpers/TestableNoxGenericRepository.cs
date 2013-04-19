using Moq;
using Nox.Tests.Helpers.Entities;

namespace Nox.Tests.Helpers
{
    public class TestableNoxGenericRepository : NoxGenericRepository<TestEntity1>
    {
        public Mock<INox> MockNox { get; set; }

        public TestableNoxGenericRepository(Mock<INox> mockNox) : base(mockNox.Object)
        {
            MockNox = mockNox;
        }

        public static TestableNoxGenericRepository Create()
        {
            return new TestableNoxGenericRepository(new Mock<INox>());
        }
    }
}