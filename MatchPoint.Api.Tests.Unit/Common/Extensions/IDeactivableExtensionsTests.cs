using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Tests.Unit.Helpers;

namespace MatchPoint.Api.Tests.Unit.Common.Extensions
{
    [TestClass]
    public class IDeactivableExtensionsTests
    {
        [TestMethod]
        public void IsActive_WithActiveStatusActive_ShouldReturnTrue()
        {
            // Arrange
            DeactivableEntityTest deactivableEntity = new()
            { 
                ActiveStatus = ActiveStatus.Active
            };

            // Act
            var result = deactivableEntity.IsActive();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsActive_WithActiveStatusInactive_ShouldReturnFalse()
        {
            // Arrange
            DeactivableEntityTest deactivableEntity = new()
            {
                ActiveStatus = ActiveStatus.Inactive
            };

            // Act
            var result = deactivableEntity.IsActive();

            // Assert
            Assert.IsFalse(result);
        }
    }
}
