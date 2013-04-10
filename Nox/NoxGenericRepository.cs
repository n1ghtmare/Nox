using System;
using System.Collections.Generic;
using System.Text;

namespace Nox
{
    public class NoxGenericRepository<T> : INoxGenericRepository<T> where T : class, new()
    {
        private readonly INox _nox;

        private string _parameterValuesFlat;
        private string _parameterKeysFlat;

        public NoxGenericRepository(INox nox)
        {
            _nox = nox;

            PopulateQuerySegments();
        }

        private void PopulateQuerySegments()
        {
            var queryValues = new StringBuilder();
            var queryKeys   = new StringBuilder();

            foreach (var property in (typeof(T)).GetProperties())
            {
                queryKeys.AppendFormat("{0}, ", property.Name);
                queryValues.AppendFormat("@{0}, ", property.Name);
            }

            _parameterKeysFlat   = FlattenQuerySegments(queryKeys);
            _parameterValuesFlat = FlattenQuerySegments(queryValues);
        }

        private string FlattenQuerySegments(StringBuilder queryParameters)
        {
            string flatParams = queryParameters.ToString().Trim();

            if (string.IsNullOrEmpty(flatParams))
                throw new Exception("Insert parameters can't be empty, make sure your entity has properties");

            return flatParams.Substring(0, flatParams.Length - 1);
        }

        public IEnumerable<T> Get(string where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(string where, object parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            var selectQuery = string.Format("SELECT {0} FROM {1}", _parameterKeysFlat, (typeof(T)).Name);

            return _nox.Execute<T>(selectQuery);
        }

        public void Create(T entity)
        {
            string insertQuery = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", entity.GetType().Name, _parameterKeysFlat, _parameterValuesFlat);

            _nox.Execute(insertQuery, entity);
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
