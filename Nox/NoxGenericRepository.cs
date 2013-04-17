﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Nox
{
    public class NoxGenericRepository<T> : INoxGenericRepository<T> where T : class, new()
    {
        private readonly INox _nox;

        public NoxGenericRepository(INox nox)
        {
            _nox = nox;
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
            var queryValues  = new StringBuilder();
            Type entityType  = entity.GetType();

            foreach (var property in entityType.GetProperties())
            {
                if (IsUsedInInsertQuery(entity, property))
                {
                    queryColumns.AppendFormat("{0}, ", property.Name);
                    queryValues.AppendFormat("@{0}, ", property.Name);
                }
            }
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                 entityType.Name, FlattenQuerySegments(queryColumns), FlattenQuerySegments(queryValues));
        }

        private bool IsUsedInInsertQuery(T entity, PropertyInfo property)
        {
            // TODO - move the primary key checkup in the constructor
            if (property.Name == "Id" ||
                property.Name == string.Format("{0}Id", typeof(T).Name) ||
                property.Name == string.Format("{0}Guid", typeof(T).Name))
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

            if (string.IsNullOrEmpty(flatParams))
                throw new Exception("Query parameters can't be empty, make sure your entity has properties");

            return flatParams.Substring(0, flatParams.Length - 1);
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            // TODO ...
            _nox.Execute("DELETE FROM TestEntity WHERE TestEntityId = @TestEntityId", entity);
        }
    }
}
