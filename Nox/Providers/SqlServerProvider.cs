using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Nox.Interfaces;

namespace Nox.Providers
{
    public sealed class SqlServerProvider : IProvider
    {
        private readonly string _connectionString;

        public int CommandTimeout { get; set; }

        public SqlServerProvider()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            CommandTimeout = 30;
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
            return new SqlCommand(query, connection as SqlConnection)
            {
                CommandType = commandType,
                CommandTimeout = CommandTimeout
            };
        }

        public IEnumerable<IDataParameter> CreateParameters(IDictionary<string, object> parameters)
        {
            return parameters.Select(parameter => new SqlParameter
            {
                ParameterName = string.Format("@{0}", parameter.Key),
                Value = parameter.Value
            });
        }
    }
}
