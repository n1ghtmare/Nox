using System;
using System.Data;
using Moq;

namespace Nox.Tests.Helpers
{
    public class TestableNox : Nox
    {
        public const string QueryScalar = "spThatReturnsASingleResult";
        public const string QueryScalarWithParameters = QueryScalar + " @TestFirstParameter = @TestFirstParameter and TestSecondParameter = @TestSecondParameter";
        public const string Query = "select * from TestTable";
        public const string QueryWithParameters = Query + " where TestFirstParameter = @TestFirstParameter and TestSecondParameter = @TestSecondParameter";

        public Mock<INoxProvider> MockNoxProvider { get; set; }

        public TestableNox(Mock<INoxProvider> mockNoxProvider)
            : base(mockNoxProvider.Object)
        {
            MockNoxProvider = mockNoxProvider;
        }

        public static TestableNox Create()
        {
            var mockNoxProvider = new Mock<INoxProvider>();
            var mockCommand = new Mock<IDbCommand>();

            mockCommand
                .Setup(x => x.ExecuteScalar())
                .Returns(0);

            mockNoxProvider
                .Setup(x => x.CreateConnection())
                .Returns(new Mock<IDbConnection>().Object);

            mockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>(), false))
               .Returns(mockCommand.Object);

            return new TestableNox(mockNoxProvider);
        }

        public static Mock<IDataReader> CreateDataReader()
        {
            var mockDataReader = new Mock<IDataReader>();
            var readToggle = true;

            mockDataReader.Setup(x => x.Read()).Returns(() => readToggle).Callback(() => readToggle = false);
            mockDataReader.Setup(x => x.FieldCount).Returns(3);
            
            mockDataReader.Setup(x => x.GetName(0)).Returns("TestPropertyString");
            mockDataReader.Setup(x => x[0]).Returns("TestResult");

            mockDataReader.Setup(x => x.GetName(1)).Returns("TestPropertyInt");
            mockDataReader.Setup(x => x[1]).Returns(1);            
            
            mockDataReader.Setup(x => x.GetName(2)).Returns("TestPropertyDateTime");
            mockDataReader.Setup(x => x[2]).Returns(DateTime.Today);

            return mockDataReader;
        }
    }
}
