using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nox
{
    public class NoxGenericRepository<T> : INoxGenericRepository<T> where T : class
    {
        private readonly Nox _nox;

        public NoxGenericRepository(Nox nox)
        {
            _nox = nox;
        }

        public object Create(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(string @where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(string @where, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
