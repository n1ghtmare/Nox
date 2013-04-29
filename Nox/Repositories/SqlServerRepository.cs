using Nox.Interfaces;
using Nox.Providers;
using Nox.QueryComposers;

namespace Nox.Repositories
{
    public class SqlServerRepository<T> : Repository<T> where T : class, new()
    {
        public SqlServerRepository()
            : this(new Conductor(new SqlServerProvider()), new SqlServerQueryComposer()) { }

        private SqlServerRepository(IConductor conductor, IQueryComposer queryComposer)
            : base(conductor, queryComposer) { }
    }
}
