using Moq;

namespace Nox.Tests.Helpers
{
    public class TestableNoxGenericRepository : NoxGenericRepository<TestEntity>
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