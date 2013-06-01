using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using NUnit.Framework;

using Nox.Providers;

namespace Nox.Tests.Providers.SqlServerProviderTests
{
    [TestFixture]
    public class CreateParameters
    {
        [Test]
        public void DictionaryWithParametersValue_ReturnsCorrectlyComposedSqlParameters()
        {
            // Arrange
            var sqlServerProvider = new SqlServerProvider();
            var parametersDictionary = new Dictionary<string, object>
            {
                {"TestParameterInt", 123},
                {"TestParameterString", "TestStringValue"}
            };

            // Act
            IEnumerable<IDataParameter> parameters = sqlServerProvider.CreateParameters(parametersDictionary);

            // Assert
            var sqlParameterInt = parameters.ElementAt(0) as SqlParameter;
            var sqlParameterString = parameters.ElementAt(1) as SqlParameter;

            Assert.IsNotNull(sqlParameterInt);
            Assert.AreEqual("@TestParameterInt", sqlParameterInt.ParameterName);
            Assert.AreEqual(123, sqlParameterInt.Value);

            Assert.IsNotNull(sqlParameterString);
            Assert.AreEqual("@TestParameterString", sqlParameterString.ParameterName);
            Assert.AreEqual("TestStringValue", sqlParameterString.Value);
        }
    }
}
