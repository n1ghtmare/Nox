using System;
using System.Collections.Generic;
using System.Text;

namespace Nox
{
    public class NoxGenericRepository<T> : INoxGenericRepository<T> where T : class
    {
        private readonly INox _nox;

        public NoxGenericRepository(INox nox)
        {
            _nox = nox;
        }

        public void Create(T entity)
        {
            var queryValues = new StringBuilder();
            var queryKeys   = new StringBuilder();
            Type entityType = entity.GetType();

            foreach (var property in entityType.GetProperties())
            {
                queryKeys.AppendFormat("{0}, ", property.Name);
                queryValues.AppendFormat("@{0}, ", property.Name);
            }

            var insertBase = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                           entityType.Name,
                                           TrimInsertQueryArguments(queryKeys),
                                           TrimInsertQueryArguments(queryValues));

            _nox.Execute(insertBase, entity);
        }

        private static string TrimInsertQueryArguments(StringBuilder queryParameters)
        {
            string flatParams = queryParameters.ToString().Trim();

            if (string.IsNullOrEmpty(flatParams))
                throw new Exception("Insert parameters can't be empty, make sure your entity has properties");

            return flatParams.Substring(0, flatParams.Length - 1);
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(string where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(string where, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
