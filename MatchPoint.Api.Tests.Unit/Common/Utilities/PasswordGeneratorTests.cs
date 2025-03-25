using MatchPoint.Api.Shared.Common.Utilities;

namespace MatchPoint.Api.Tests.Unit.Common.Utilities
{
    [TestClass]
    public class PasswordGeneratorTests
    {
        [TestMethod]
        public void GenerateNumeric_WithValidDigits_ShouldGeneratePwdOfCorrectLength()
        {
            // Arrange
            int pwdLength = 5;

            // Act
            var result = PasswordGenerator.GenerateNumeric(pwdLength);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pwdLength, result.Length);
        }

        [TestMethod]
        public void GenerateNumeric_WithPrefix_ShouldGeneratePwdWithPrefix()
        {
            // Arrange
            int pwdLength = 5;
            string prefix = "UnitTests_";

            // Act
            var result = PasswordGenerator.GenerateNumeric(pwdLength, prefix);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pwdLength + prefix.Length, result.Length);
            Assert.IsTrue(result.StartsWith(prefix));
        }
    }
}
