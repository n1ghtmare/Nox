using Nox.Interfaces;
using Nox.Providers;
using Nox.QueryComposers;

namespace Nox.Repositories
{
    public class SqlServerRepository<T> : Repository<T> where T : class, new()
    {
        public SqlServerRepository()
            : this(new Nox(new SqlServerProvider()), new SqlServerQueryComposer()) { }

        private SqlServerRepository(INox nox, IQueryComposer queryComposer)
            : base(nox, queryComposer) { }
    }
}
