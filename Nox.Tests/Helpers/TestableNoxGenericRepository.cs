using Moq;

namespace Nox.Tests.Helpers
{
    public class TestableNoxGenericRepository<T> : NoxGenericRepository<T> where T : class, new()
    {
        public Mock<INox> MockNox { get; set; }

        public TestableNoxGenericRepository(Mock<INox> mockNox) : base(mockNox.Object)
        {
            MockNox = mockNox;
        }

        public static TestableNoxGenericRepository<T> Create()
        {
            return new TestableNoxGenericRepository<T>(new Mock<INox>());
        }
    }
}