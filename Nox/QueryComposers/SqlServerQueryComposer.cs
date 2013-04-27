using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nox.Interfaces;

namespace Nox.QueryComposers
{
    public class SqlServerQueryComposer : IQueryComposer
    {
        public string ComposeInsert(Type entityType)
        {
            return ComposeInsert(entityType, entityType.GetProperties());
        }

        public string ComposeInsert(Type entityType, string primaryKeyName)
        {
            IEnumerable<PropertyInfo> includedProperties =
                entityType.GetProperties().Where(property => property.Name != primaryKeyName);
            
            string selectQuery = ComposeInsert(entityType, includedProperties);
            return string.Format("{0} SELECT SCOPE_IDENTITY()", selectQuery);
        }

        private string ComposeInsert(Type entityType, IEnumerable<PropertyInfo> includedProperties)
        {
            var colSegments = new StringBuilder();
            var valSegments = new StringBuilder();

            foreach (var property in includedProperties)
            {
                colSegments.AppendFormat("{0}, ", property.Name);
                valSegments.AppendFormat("@{0}, ", property.Name);
            }
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                 entityType.Name, FlattenQuerySegments(colSegments), FlattenQuerySegments(valSegments));
        }

        private static string FlattenQuerySegments(StringBuilder queryParameters)
        {
            string flatParams = queryParameters.ToString().Trim();
            return flatParams.Substring(0, flatParams.Length - 1);
        }

        public string ComposeDelete(Type entityType, string primaryKeyName)
        {
            return string.Format("DELETE FROM {0} WHERE {1} = @{1}", entityType.Name, primaryKeyName);
        }

        public string ComposeUpdate(Type entityType, string primaryKeyName)
        {
            var updateSegments = new StringBuilder();

            foreach (var property in entityType.GetProperties().Where(property => property.Name != primaryKeyName))
                updateSegments.AppendFormat("{0} = @{0}, ", property.Name);

            return string.Format("UPDATE {0} SET {1} WHERE {2} = @{2}",
                                 entityType.Name, FlattenQuerySegments(updateSegments), primaryKeyName);
        }

        public string ComposeSelect(Type entityType)
        {
            var queryColumns = new StringBuilder();

            foreach (var property in entityType.GetProperties())
                queryColumns.AppendFormat("{0}, ", property.Name);

            return string.Format("SELECT {0} FROM {1}", FlattenQuerySegments(queryColumns), entityType.Name);
        }
    }
}