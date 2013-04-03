using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

using NUnit.Framework;

namespace Nox.Tests.Integration
{
    public class Execute
    {
        [Test]
        public void GivenAQueryWithParameters_ReturnsNotNull()
        {
            // Arrange
            var provider = new SqlServerNoxProvider();
            var nox = new Nox(provider);

            var query = "select * from Accounts where FirstName = @FirstName and LastName = @LastName";
            var parameters = new { FirstName = "Dimitar", LastName = "Dimitrov"};

            // Act
            var results = nox.Execute(query: query, parameters: parameters).ToList();

            // Assert
            Assert.IsNotNull(results.First());
            Assert.AreEqual(1, results.Count);
        }
    }


    public class Employee
    {
        
    }

    public interface INoxProvider
    {
        IEnumerable<dynamic> Execute(string query, object parameters = null);
    }

    public class SqlServerNoxProvider : INoxProvider
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

        public IEnumerable<object> Execute(string query, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                AppendParameters(parameters, command);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        yield return ComposeExpandoObject(reader);
                }
            }
        }

        private static void AppendParameters(object parameters, SqlCommand command)
        {
            if (parameters != null)
            {
                foreach (var propertyInfo in parameters.GetType().GetProperties())
                {
                    string propertyName = propertyInfo.Name;
                    string propertyValue = propertyInfo.GetValue(parameters, null).ToString();

                    command.Parameters.AddWithValue(string.Format("@{0}", propertyName), propertyValue);
                }
            }
        }

        private static IDictionary<string, object> ComposeExpandoObject(SqlDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (int i = 0; i < reader.FieldCount; i++)
                expandoObject.Add(reader.GetName(i), reader[i]);

            return expandoObject;
        }
    }

    public class Nox
    {
        private readonly INoxProvider _provider;

        public Nox(INoxProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<dynamic> Execute(string query, object parameters = null)
        {
            return _provider.Execute(query, parameters);
        }
    }
}
