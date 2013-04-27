﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Nox.Helpers;
using Nox.Interfaces;

namespace Nox.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly INox _nox;
        private readonly IQueryComposer _queryComposer;
        private readonly PropertyInfo _primaryKeyProperty;
        private readonly QueryCache _queryCache;

        public Repository(INox nox, IQueryComposer queryComposer)
        {
            _nox = nox;
            _queryComposer = queryComposer;
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
            string selectQuery = ComposeAndCacheSelectQuery();
            return _nox.Execute<T>(selectQuery);
        }

        public IEnumerable<T> Get(string where, object parameters)
        {
            if(parameters == null)
                throw new ArgumentNullException("parameters", "Can't pass null parameters, make sure you pass valid query parameters");

            string selectQuery = ComposeAndCacheSelectQuery();
            return _nox.Execute<T>(string.Format("{0} WHERE {1}", selectQuery, where), parameters);
        }

        private string ComposeAndCacheSelectQuery()
        {
            if (string.IsNullOrEmpty(_queryCache.Select))
                _queryCache.Select = _queryComposer.ComposeSelect(typeof(T));
            return _queryCache.Select;
        }

        public void Create(T entity)
        {
            string insertQuery = ComposeAndCacheInsertQuery(entity);
            var id = _nox.ExecuteScalar<object>(insertQuery, entity);

            _primaryKeyProperty.SetValue(entity, id);
        }

        private string ComposeAndCacheInsertQuery(T entity)
        {
            return PrimaryKeyHasValue(entity) ? GetCachedInsertQueryWithPk() : GetCachedInsertQuery();
        }

        private string GetCachedInsertQuery()
        {
            if (string.IsNullOrEmpty(_queryCache.Insert))
                _queryCache.Insert = _queryComposer.ComposeInsert(typeof(T), _primaryKeyProperty.Name);

            return _queryCache.Insert;
        }

        private string GetCachedInsertQueryWithPk()
        {
            if (string.IsNullOrEmpty(_queryCache.InsertWithPk))
                _queryCache.InsertWithPk = _queryComposer.ComposeInsert(typeof(T));

            return _queryCache.InsertWithPk;
        }

        private bool PrimaryKeyHasValue(T entity)
        {
            object propertyValue = _primaryKeyProperty.GetValue(entity, null);

            return propertyValue != null && propertyValue.ToString() != "0" &&
                   (!(propertyValue is Guid) || (Guid) propertyValue != Guid.Empty);
        }

        public void Update(T entity)
        {
            if (_primaryKeyProperty == null)
                throw new Exception("Can't compose an update query - unable to detect primary key");

            if (string.IsNullOrEmpty(_queryCache.Update))
                _queryCache.Update = _queryComposer.ComposeUpdate(typeof(T), _primaryKeyProperty.Name);

            _nox.Execute(_queryCache.Update, entity);
        }

        public void Delete(T entity)
        {
            if (_primaryKeyProperty == null)
                throw new Exception("Can't compose a delete query - unable to detect primary key");

            if (string.IsNullOrEmpty(_queryCache.Delete))
                _queryCache.Delete = _queryComposer.ComposeDelete(typeof (T), _primaryKeyProperty.Name);

            _nox.Execute(_queryCache.Delete, entity);
        }
    }
}