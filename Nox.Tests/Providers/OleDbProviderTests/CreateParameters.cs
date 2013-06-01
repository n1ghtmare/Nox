using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

using NUnit.Framework;

using Nox.Providers;

namespace Nox.Tests.Providers.OleDbProviderTests
{
    [TestFixture]
    public class CreateParameters
    {
        [Test]
        public void DictionaryWithParametersValue_ReturnsCorrectlyComposedOleDbParameters()
        {
            // Arrange
            var sqlServerProvider = new OleDbProvider("fakeConnectionString");
            var parametersDictionary = new Dictionary<string, object>
            {
                {"TestParameterInt", 123},
                {"TestParameterString", "TestStringValue"}
            };

            // Act
            IEnumerable<IDataParameter> parameters = sqlServerProvider.CreateParameters(parametersDictionary);

            // Assert
            var oleDbParameterInt = parameters.ElementAt(0) as OleDbParameter;
            var oleDbParameterString = parameters.ElementAt(1) as OleDbParameter;

            Assert.IsNotNull(oleDbParameterInt);
            Assert.AreEqual("?", oleDbParameterInt.ParameterName);
            Assert.AreEqual(123, oleDbParameterInt.Value);

            Assert.IsNotNull(oleDbParameterString);
            Assert.AreEqual("?", oleDbParameterString.ParameterName);
            Assert.AreEqual("TestStringValue", oleDbParameterString.Value);
        }
    }
}
