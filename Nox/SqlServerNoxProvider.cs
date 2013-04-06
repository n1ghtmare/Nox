using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Nox
{
    public sealed class SqlServerNoxProvider : INoxProvider
    {
        private readonly string _connectionString;

        public SqlServerNoxProvider()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
        }

        public SqlServerNoxProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbCommand CreateCommand(string query, IDbConnection connection, bool isStoredProcedure)
        {
            var command = new SqlCommand(query, connection as SqlConnection);
            
            if(isStoredProcedure)
                command.CommandType = CommandType.StoredProcedure;

            return command;
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
