using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Nox.Interfaces;

namespace Nox.Providers
{
    public sealed class SqlServerProvider : IProvider
    {
        private readonly string _connectionString;

        public SqlServerProvider()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
        }

        public SqlServerProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbCommand CreateCommand(string query, IDbConnection connection, CommandType commandType)
        {
            return new SqlCommand(query, connection as SqlConnection) { CommandType = commandType };
        }

        public IEnumerable<IDataParameter> CreateParameters(object parameters)
        {
            if (parameters != null)
            {
                foreach (var propertyInfo in parameters.GetType().GetProperties())
                {
                    var sqlParameter = new SqlParameter
                    {
                        ParameterName = string.Format("@{0}", propertyInfo.Name),
                        Value = propertyInfo.GetValue(parameters, null)
                    };
                    yield return sqlParameter;
                }
            }
        }
    }
}
