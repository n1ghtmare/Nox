using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Nox.Tests
{
    public class Something
    {
        [Test]
        public void DynamicObject_CallsSomething()
        {
            // Arrange
            var table = new Employees();


            // Act
            table.Insert(new {Id = "TEST", FirstName = "Dimitar", LastName = "Dimitrov"});

            // Assert
        }
    }
}
