using System.Collections.Generic;

namespace Nox
{
    public interface INoxGenericRepository<T> where T : class
    {
        object Create(T entity);
        void Delete(T entity);
        void Update(T entity);
        IEnumerable<T> Get(string where);
        IEnumerable<T> Get(string where, object[] parameters);
        IEnumerable<T> GetAll();
    }
}
