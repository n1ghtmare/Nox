using Moq;

using Nox.Interfaces;
using Nox.Repositories;

namespace Nox.Tests.Helpers
{
    public class TestableRepository<T> : Repository<T> where T : class, new()
    {
        public Mock<IConductor> MockConductor { get; set; }
        public Mock<IQueryComposer> MockQueryComposer { get; set; }

        public TestableRepository(Mock<IConductor> mockConductor, Mock<IQueryComposer> mockQueryComposer) 
            : base(mockConductor.Object, mockQueryComposer.Object)
        {
            MockConductor = mockConductor;
            MockQueryComposer = mockQueryComposer;
        }

        public static TestableRepository<T> Create()
        {
            return new TestableRepository<T>(new Mock<IConductor>(), new Mock<IQueryComposer>());
        }
    }
}