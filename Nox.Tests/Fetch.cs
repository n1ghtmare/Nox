using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;

namespace Nox.Tests
{
    public class Get
    {
        [Test]
        public void GivenAWhereClauseAndParameters_CallsProviderExecuteQueryWithCorrectSqlQuery()
        {
            // Arrange
            var provider = new Mock<INoxProvider>();
            var nox = new Nox(provider.Object);

            // Act
            IEnumerable<Employee> employeeList = nox.Get<Employee>(where: "FirstName=@FirstName and LastName=@LastName",
                                                                   param:
                                                                       new
                                                                           {
                                                                               FirstName = "Dimitar",
                                                                               LastName = "Dimitrov"
                                                                           });

            
            // Assert
            provider.Verify(
                x => x.ExecuteQuery("SELECT * FROM Employee WHERE FirstName='Dimitar' and LastName='Dimitrov'"),
                Times.Once());
        }
    }


    public class Employee
    {
        
    }

    public interface INoxProvider
    {
//        string ConnectionString { get; set; }
        object ExecuteQuery(string sql);
    }


    public class Nox
    {
        private INoxProvider _provider;

        public Nox(INoxProvider provider)
        {
            _provider = provider;
        }
        public IEnumerable<T> GetAll<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get<T>(string where = "", object param = null)
        {
            string sqlQuery = "SELECT * FROM {0} WHERE {1}";
            // Build select query -> replace parameters as well here ...
//            throw new NotImplementedException();

            return null;
        }
    }
}
