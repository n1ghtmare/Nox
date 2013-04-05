using System.Data;
using Moq;

namespace Nox.Tests
{
    public class TestableNox : Nox
    {
        public Mock<INoxProvider> MockNoxProvider { get; set; }

        public TestableNox(Mock<INoxProvider> mockNoxProvider)
            : base(mockNoxProvider.Object)
        {
            MockNoxProvider = mockNoxProvider;
        }

        public static TestableNox Create()
        {
            var mockNoxProvider = new Mock<INoxProvider>();

            mockNoxProvider
                .Setup(x => x.CreateConnection())
                .Returns(new Mock<IDbConnection>().Object);

            mockNoxProvider
               .Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>()))
               .Returns(new Mock<IDbCommand>().Object);

            return new TestableNox(mockNoxProvider);
        }
    }
}
