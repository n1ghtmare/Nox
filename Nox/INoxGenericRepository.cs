using System.Collections.Generic;

namespace Nox
{
    public interface INoxGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> Get(string where, object parameters);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
