using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

using Moq;
using NUnit.Framework;

using Nox.Tests.Helpers;

namespace Nox.Tests
{
    public class Execute
    {
        private const string Query = "select * from Accounts";
        private const string QueryWithParameters = Query + " where FirstName = @FirstName and LastName = @LastName";
        
        [Test]
        public void Query_CallsNoxProviderCreateConnection()
        {
            // Arrange
            var nox = TestableNox.Create();

            // Act
            nox.Execute<dynamic>(Query).ToList();

            // Assert
            nox.MockNoxProvider
                .Verify(x => x.CreateConnection(), Times.Once());
        }

        [Test]
        public void Query_CallsNoxProviderCreateCommand()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider
               .Setup(x => x.CreateConnection())
               .Returns(mockConnection.Object);

            // Act
            nox.Execute<dynamic>(Query).ToList();

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateCommand(Query, mockConnection.Object),
                       Times.Once());
        }

        [Test]
        public void QueryWithParameters_CallsNoxProviderCreateParameters()
        {
            // Arrange
            var nox = TestableNox.Create();
            var parameters = new {FirstName = "John", LastName = "Doe"};


            // Act
            nox.Execute<dynamic>(QueryWithParameters, parameters).ToList();

            // Assert
            nox.MockNoxProvider
               .Verify(x => x.CreateParameters(parameters),
                       Times.Once());
        }

        [Test]
        public void QueryWithParameters_CallsNoxProviderCommandAddParameterForEachProvidedParameter()
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
            nox.Execute<dynamic>(QueryWithParameters, parameters).ToList();

            // Assert
            mockParameterCollection
                .Verify(x => x.Add(It.IsAny<IDataParameter>()),
                Times.Exactly(2));
        }

        [Test]
        public void Query_CallsNoxProviderConnectionOpen()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockConnection = new Mock<IDbConnection>();

            nox.MockNoxProvider
               .Setup(x => x.CreateConnection())
               .Returns(mockConnection.Object);

            // Act
            nox.Execute<dynamic>(Query).ToList();

            // Assert
            mockConnection.Verify(x => x.Open(), Times.Once());
        }

        [Test]
        public void Query_CallsNoxProviderCommandExecuteReader()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockCommand = new Mock<IDbCommand>();

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>()))
               .Returns(mockCommand.Object);
            
            // Act
            nox.Execute<dynamic>(Query).ToList();

            // Assert
            mockCommand.Verify(x => x.ExecuteReader(), Times.Once());
        }

        [Test]
        public void QueryThatFindsResults_ReturnsAListOfDynamicResultsWithCorrectCount()
        {
            // Arrange
            var nox = TestableNox.Create(); 
            var mockDataReader = TestableNox.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>()))
               .Returns(mockCommand.Object);

            // Act
            IEnumerable<dynamic> results = nox.Execute<dynamic>(Query).ToList();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("TestResult", results.First().TestPropertyString);
        }



        [Test]
        public void QueryAndType_ReturnsCorrectlyMappedType()
        {
            // Arrange
            var nox = TestableNox.Create();
            var mockDataReader = TestableNox.CreateDataReader();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteReader()).Returns(mockDataReader.Object);

            nox.MockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>()))
               .Returns(mockCommand.Object);

            // Act
            IEnumerable<TestEntity> results = nox.Execute<TestEntity>(Query).ToList();

            // Assert
            TestEntity testEntity = results.First();
            Assert.AreEqual(testEntity.TestPropertyDateTime, DateTime.Today);
            Assert.AreEqual(testEntity.TestPropertyInt, 1);
            Assert.AreEqual(testEntity.TestPropertyString, "Test_String");
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

        public IEnumerable<T> Execute<T>(string query, object parameters = null) where T : new()
        {
            using (IDbConnection connection = _provider.CreateConnection())
            using (IDbCommand command = _provider.CreateCommand(query, connection))
            {
                IEnumerable<IDataParameter> generatedParameters = _provider.CreateParameters(parameters);

                foreach (IDataParameter parameter in generatedParameters)
                    command.Parameters.Add(parameter);

                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        dynamic entity = new ExpandoObject();

                        for (int i = 0; i < reader.FieldCount; i++)
                            ((IDictionary<string, object>)entity)[reader.GetName(i)] = reader[i];

                        yield return entity;
                    }
                }
            }
        }

//        public IEnumerable<T> Execute<T>(string query, object parameters = null)
//        {
//            IEnumerable<dynamic> results = Execute(query, parameters);
//
//            Type currentType = typeof (T);
//
//
//            return null;
//        }

        private static IDictionary<string, object> ComposeExpandoObject(IDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (int i = 0; i < reader.FieldCount; i++)
                expandoObject.Add(reader.GetName(i), reader[i]);

            return expandoObject;
        }
    }
}
