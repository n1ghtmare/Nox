using System.Collections.Generic;

namespace Nox
{
    public interface INoxGenericRepository<T> where T : class
    {
        IEnumerable<T> Get(string where);
        IEnumerable<T> Get(string where, object parameters);
        IEnumerable<T> GetAll();
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
