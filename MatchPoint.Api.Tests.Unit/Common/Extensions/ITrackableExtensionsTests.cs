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
            #region Arrange
            TrackableEntityTest trackableEntity = new();
            #endregion

            #region Act
            trackableEntity.SetTrackingFields(updating: false);
            #endregion

            #region Assert
            Assert.AreNotEqual(default, trackableEntity.CreatedBy);
            Assert.AreNotEqual(default, trackableEntity.CreatedOnUtc);
            Assert.IsNull(trackableEntity.ModifiedBy);
            Assert.IsNull(trackableEntity.ModifiedOnUtc);
            #endregion
        }

        [TestMethod]
        public void SetTrackingFields_WhenUpdatingIsTrue_ShouldSetUpdatedFields()
        {
            #region Arrange
            TrackableEntityTest trackableEntity = new();
            #endregion

            #region Act
            trackableEntity.SetTrackingFields(updating: true);
            #endregion

            #region Assert
            Assert.AreNotEqual(default, trackableEntity.ModifiedBy);
            Assert.AreNotEqual(default, trackableEntity.ModifiedOnUtc);
            Assert.AreEqual(default, trackableEntity.CreatedBy);
            Assert.AreEqual(default, trackableEntity.CreatedOnUtc);
            #endregion
        }
    }
}
