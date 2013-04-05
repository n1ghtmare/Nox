using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Nox.Tests
{
    public class Execute
    {
//        [Test]
//        public void GivenAQueryWithParameters_ReturnsNotNull()
//        {
//            // Arrange
//            var provider = new SqlServerNoxProvider();
//            var nox = new Nox(provider);
//
//            var query = "select * from Accounts where FirstName = @FirstName and LastName = @LastName";
//            var parameters = new { FirstName = "Dimitar", LastName = "Dimitrov"};
//
//            // Act
//            var results = nox.Execute(query: query, parameters: parameters).ToList();
//
//            // Assert
//            Assert.IsNotNull(results.First());
//            Assert.AreEqual(1, results.Count);
//        }

        private const string Query = "select * from Accounts";
        private const string QueryWithParameters = Query + " where FirstName = @FirstName and LastName = @LastName";
        
        [Test]
        public void GivenAQuery_CallsNoxProviderCreateConnection()
        {
            // Arrange
            var nox = TestableNox.Create();

            // Act
            nox.Execute(query: Query);

            // Assert
            nox.MockNoxProvider
                .Verify(x => x.CreateConnection(), Times.Once());
        }

        [Test]
        public void GivenAQuery_CallsNoxProviderCreateCommand()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider
               .Setup(x => x.CreateConnection())
               .Returns(mockConnection.Object);

            // Act
            nox.Execute(query: Query);

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateCommand(Query, mockConnection.Object),
                       Times.Once());
        }

        [Test]
        public void GivenAQueryWithParameters_CallsNoxProviderCreateParameters()
        {
            // Arrange
            var nox = TestableNox.Create();
            var parameters = new {FirstName = "John", LastName = "Doe"};


            // Act
            nox.Execute(query: QueryWithParameters, parameters: parameters);

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateParameters(parameters),
                       Times.Once());
        }

        [Test]
        public void GivenAQueryWithParameters_CallsNoxProviderCommandAddParameterForEachProvidedParameter()
        {
            // Arrange
            var nox = TestableNox.Create();
            var parameters = new {FirstName = "John", LastName = "Doe"};
            var mockCommand = new Mock<IDbCommand>();
            var mockParameterCollection = new Mock<IDataParameterCollection>();

            mockCommand.Setup(x => x.Parameters)
                       .Returns(mockParameterCollection.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>()))
               .Returns(mockCommand.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateParameters(It.IsAny<object>()))
               .Returns(new List<IDbDataParameter>
                   {
                       new SqlParameter("@FirstName", "John"), 
                       new SqlParameter("@LastName", "Doe")
                   });
            
            // Act
            nox.Execute(query: QueryWithParameters, parameters: parameters);

            // Assert
            mockParameterCollection
                .Verify(x => x.Add(It.IsAny<IDataParameter>()),
                Times.Exactly(2));
        }

        [Test]
        public void GivenAQuery_CallsNoxProviderConnectionOpen()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider
               .Setup(x => x.CreateConnection())
               .Returns(mockConnection.Object);

            // Act
            nox.Execute(Query);

            // Assert
            mockConnection.Verify(x => x.Open(), Times.Once());
        }
    }

    public interface INoxProvider
    {
        //IEnumerable<dynamic> Execute(string query, object parameters = null);

        IDbConnection CreateConnection();
        IDbCommand CreateCommand(string query, IDbConnection dbConnection);
        IEnumerable<IDataParameter> CreateParameters(object parameters);
    }

//    public class SqlServerNoxProvider : INoxProvider
//    {
//        private readonly string _connectionString;
//
//        public SqlServerNoxProvider()
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
//        }
//
//        public SqlServerNoxProvider(string connectionString)
//        {
//            _connectionString = connectionString;
//        }
//
//        public IEnumerable<object> Execute(string query, object parameters = null)
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            using (var command = new SqlCommand(query, connection))
//            {
//                AppendParameters(parameters, command);
//
//                connection.Open();
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                        yield return ComposeExpandoObject(reader);
//                }
//            }
//        }
//
//        private static void AppendParameters(object parameters, SqlCommand command)
//        {
//            if (parameters != null)
//            {
//                foreach (var propertyInfo in parameters.GetType().GetProperties())
//                {
//                    string propertyName = propertyInfo.Name;
//                    string propertyValue = propertyInfo.GetValue(parameters, null).ToString();
//
//                    command.Parameters.AddWithValue(string.Format("@{0}", propertyName), propertyValue);
//                }
//            }
//        }
//
//        private static IDictionary<string, object> ComposeExpandoObject(IDataRecord reader)
//        {
//            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
//
//            for (int i = 0; i < reader.FieldCount; i++)
//                expandoObject.Add(reader.GetName(i), reader[i]);
//
//            return expandoObject;
//        }
//    }

    public class Nox
    {
        private readonly INoxProvider _provider;

        public Nox(INoxProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<dynamic> Execute(string query, object parameters = null)
        {
            using (IDbConnection connection = _provider.CreateConnection())
            using (IDbCommand command = _provider.CreateCommand(query, connection))
            {
                IEnumerable<IDataParameter> generatedParameters = _provider.CreateParameters(parameters);

                foreach (var param in generatedParameters)
                    command.Parameters.Add(param);

                connection.Open();
            }


            return null;
        }
    }
}
