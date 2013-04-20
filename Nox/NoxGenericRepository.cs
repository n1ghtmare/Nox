using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nox
{
    public class NoxGenericRepository<T> : INoxGenericRepository<T> where T : class, new()
    {
        private readonly INox _nox;
        private readonly PropertyInfo _primaryKeyProperty;
        private readonly QueryCache _queryCache;

        public NoxGenericRepository(INox nox)
        {
            _nox = nox;
            _primaryKeyProperty = GetPrimaryKeyProperty();
            _queryCache = new QueryCache();
        }

        private static PropertyInfo GetPrimaryKeyProperty()
        {
            Type entityType = typeof (T);
            return entityType.GetProperties().FirstOrDefault(
                property => property.Name == "Id" ||
                            property.Name == string.Format("{0}Id", entityType.Name) ||
                            property.Name == string.Format("{0}Guid", entityType.Name));
        }

        public IEnumerable<T> GetAll()
        {
            if (string.IsNullOrEmpty(_queryCache.Select))
                _queryCache.Select = ComposeSelectQuery();

            return _nox.Execute<T>(_queryCache.Select);
        }

        private string ComposeSelectQuery()
        {
            var queryColumns = new StringBuilder();
            var entityType = typeof (T);

            foreach (var property in entityType.GetProperties())
                queryColumns.AppendFormat("{0}, ", property.Name);

            return string.Format("SELECT {0} FROM {1}", FlattenQuerySegments(queryColumns), entityType.Name);
        }

        public IEnumerable<T> Get(string where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(string where, object parameters)
        {
            throw new NotImplementedException();
        }

        public void Create(T entity)
        {
            string insertQuery = ComposeAndCacheInsertQuery(entity);
            _nox.Execute(insertQuery, entity);
        }

        private string ComposeAndCacheInsertQuery(T entity)
        {
            if (PrimaryKeyHasValue(entity))
            {
                if (string.IsNullOrEmpty(_queryCache.InsertWithPk))
                    _queryCache.InsertWithPk = ComposeInsertQueryWithPrimaryKey();
                return _queryCache.InsertWithPk;
            }

            if (string.IsNullOrEmpty(_queryCache.Insert))
                _queryCache.Insert = ComposeInsertQuery();
            return _queryCache.Insert;
        }

        private bool PrimaryKeyHasValue(T entity)
        {
            object propertyValue = _primaryKeyProperty.GetValue(entity, null);

            return propertyValue != null && propertyValue.ToString() != "0" &&
                   (!(propertyValue is Guid) || (Guid) propertyValue != Guid.Empty);
        }

        private string ComposeInsertQuery()
        {
            IEnumerable<PropertyInfo> defaultProperties =
                typeof (T).GetProperties().Where(property => property != _primaryKeyProperty);
            return ComposeInsertQuery(defaultProperties);
        }

        private string ComposeInsertQueryWithPrimaryKey()
        {
            IEnumerable<PropertyInfo> defaultProperties = typeof (T).GetProperties();
            return ComposeInsertQuery(defaultProperties);
        }

        private string ComposeInsertQuery(IEnumerable<PropertyInfo> properties)
        {
            var colSegments = new StringBuilder();
            var valSegments = new StringBuilder();

            foreach (var property in properties)
            {
                colSegments.AppendFormat("{0}, ", property.Name);
                valSegments.AppendFormat("@{0}, ", property.Name);
            }
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                 typeof (T).Name, FlattenQuerySegments(colSegments), FlattenQuerySegments(valSegments));
        }

        private static string FlattenQuerySegments(StringBuilder queryParameters)
        {
            string flatParams = queryParameters.ToString().Trim();
            return flatParams.Substring(0, flatParams.Length - 1);
        }

        public void Update(T entity)
        {
            if (string.IsNullOrEmpty(_queryCache.Update))
                _queryCache.Update = ComposeUpdateQuery();

            _nox.Execute(_queryCache.Update, entity);
        }

        private string ComposeUpdateQuery()
        {
            if (_primaryKeyProperty == null)
                throw new Exception("Can't compose an update query - unable to detect primary key");

            var entityType = typeof (T);
            var updateSegments = new StringBuilder();

            foreach (var property in entityType.GetProperties().Where(property => property != _primaryKeyProperty))
                updateSegments.AppendFormat("{0} = @{0}, ", property.Name);

            return string.Format("UPDATE {0} SET {1} WHERE {2} = @{2}",
                                 entityType.Name, FlattenQuerySegments(updateSegments), _primaryKeyProperty.Name);
        }

        public void Delete(T entity)
        {
            if (string.IsNullOrEmpty(_queryCache.Delete))
                _queryCache.Delete = ComposeDeleteQuery();

            _nox.Execute(_queryCache.Delete, entity);
        }

        private string ComposeDeleteQuery()
        {
            if (_primaryKeyProperty == null)
                throw new Exception("Can't compose a delete query - unable to detect primary key");

            return string.Format("DELETE FROM {0} WHERE {1} = @{1}",
                                 typeof (T).Name, _primaryKeyProperty.Name);
        }
    }
}
