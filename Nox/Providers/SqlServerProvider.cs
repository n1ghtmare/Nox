﻿using System.Collections.Generic;
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
            : this(ConfigurationManager.ConnectionStrings[0].ConnectionString)
        {   
        }

        public SqlServerProvider(string connectionString)
        {
            _connectionString = connectionString;
            CommandTimeout = 30;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbCommand CreateCommand(string query, IDbConnection connection, CommandType commandType)
        {
            return new SqlCommand
            {
                CommandText    = query,
                Connection     = connection as SqlConnection,
                CommandType    = commandType,
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
