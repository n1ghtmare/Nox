using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

using Nox.Interfaces;

namespace Nox.Providers
{
    public class OleDbProvider : IProvider
    {
        private readonly string _connectionString;

        public int CommandTimeout { get; set; }

        public OleDbProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new OleDbConnection(_connectionString);
        }

        public IDbCommand CreateCommand(string query, IDbConnection connection, CommandType commandType)
        {
            return new OleDbCommand
            {
                CommandText    = query,
                Connection     = connection as OleDbConnection,
                CommandType    = commandType,
                CommandTimeout = CommandTimeout
            };
        }

        public IEnumerable<IDataParameter> CreateParameters(IDictionary<string, object> parameters)
        {
            return parameters.Select(parameter => new OleDbParameter
            {
                ParameterName = "?",
                Value = parameter.Value
            });
        }
    }
}
