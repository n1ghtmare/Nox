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

        public NoxGenericRepository(INox nox)
        {
            _nox = nox;
            _primaryKeyProperty = GetPrimaryKeyProperty();
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
            string selectQuery = ComposeSelectQuery();

            return _nox.Execute<T>(selectQuery);
        }

        private string ComposeSelectQuery()
        {
            var queryColumns = new StringBuilder();
            Type entityType = typeof (T);

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
            string insertQuery = ComposeInsertQuery(entity);

            _nox.Execute(insertQuery, entity);
        }

        private string ComposeInsertQuery(T entity)
        {
            var queryColumns = new StringBuilder();
            var queryValues = new StringBuilder();
            Type entityType = entity.GetType();

            foreach (var property in entityType.GetProperties().Where(property => IsUsedInInsertQuery(entity, property)))
            {
                queryColumns.AppendFormat("{0}, ", property.Name);
                queryValues.AppendFormat("@{0}, ", property.Name);
            }
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                 entityType.Name, FlattenQuerySegments(queryColumns), FlattenQuerySegments(queryValues));
        }

        private bool IsUsedInInsertQuery(T entity, PropertyInfo property)
        {
            if (property == _primaryKeyProperty)
            {
                var propertyValue = property.GetValue(entity, null);
                if ((propertyValue == null || propertyValue.ToString() == "0") || 
                    (propertyValue is Guid && (Guid) propertyValue == Guid.Empty))
                    return false;
            }
            return true;
        }

        private string FlattenQuerySegments(StringBuilder queryParameters)
        {
            string flatParams = queryParameters.ToString().Trim();
            return flatParams.Substring(0, flatParams.Length - 1);
        }

        public void Update(T entity)
        {
            var updateQuery = ComposeUpdateQuery();

            _nox.Execute(updateQuery, entity);
        }

        private string ComposeUpdateQuery()
        {
            if(_primaryKeyProperty == null)
                throw new Exception("Can't compose an update query - unable to detect primary key");

            var entityType = typeof(T);
            var updateSegments = new StringBuilder();

            foreach (var property in entityType.GetProperties().Where(property => property != _primaryKeyProperty))
                updateSegments.AppendFormat("{0} = @{0}, ", property.Name);

            return string.Format("UPDATE {0} SET {1} WHERE {2} = @{2}",
                                            entityType.Name, FlattenQuerySegments(updateSegments), _primaryKeyProperty.Name);
        }

        public void Delete(T entity)
        {
            var deleteQuery = ComposeDeleteQuery();

            _nox.Execute(deleteQuery, entity);
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
