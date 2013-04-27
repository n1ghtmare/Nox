using Moq;

using Nox.Interfaces;
using Nox.Repositories;

namespace Nox.Tests.Helpers
{
    public class TestableRepository<T> : Repository<T> where T : class, new()
    {
        public Mock<INox> MockNox { get; set; }
        public Mock<IQueryComposer> MockQueryComposer { get; set; }

        public TestableRepository(Mock<INox> mockNox, Mock<IQueryComposer> mockQueryComposer) 
            : base(mockNox.Object, mockQueryComposer.Object)
        {
            MockNox = mockNox;
            MockQueryComposer = mockQueryComposer;
        }

        public static TestableRepository<T> Create()
        {
            return new TestableRepository<T>(new Mock<INox>(), new Mock<IQueryComposer>());
        }
    }
}