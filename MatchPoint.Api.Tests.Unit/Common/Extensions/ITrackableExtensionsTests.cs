using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Tests.Unit.Helpers;

namespace MatchPoint.Api.Tests.Unit.Common.Extensions
{
    [TestClass]
    public class ITrackableExtensionsTests
    {
        [TestMethod]
        public void SetTrackingFields_WhenUpdatingIsFalse_ShouldSetCreatedFields()
        {
            // Arrange
            TrackableEntityTest trackableEntity = new();
            Guid userId = Guid.NewGuid();

            // Act
            trackableEntity.SetTrackingFields(userId, updating: false);

            // Assert
            Assert.AreEqual(userId, trackableEntity.CreatedBy);
            Assert.AreNotEqual(default, trackableEntity.CreatedOnUtc);
            Assert.IsNull(trackableEntity.ModifiedBy);
            Assert.IsNull(trackableEntity.ModifiedOnUtc);
        }

        [TestMethod]
        public void SetTrackingFields_WhenUpdatingIsTrue_ShouldSetUpdatedFields()
        {
            // Arrange
            TrackableEntityTest trackableEntity = new();
            Guid userId = Guid.NewGuid();

            // Act
            trackableEntity.SetTrackingFields(userId, updating: true);

            // Assert
            Assert.AreEqual(userId, trackableEntity.ModifiedBy);
            Assert.AreNotEqual(default, trackableEntity.ModifiedOnUtc);
            Assert.AreEqual(default, trackableEntity.CreatedBy);
            Assert.AreEqual(default, trackableEntity.CreatedOnUtc);
        }
    }
}
