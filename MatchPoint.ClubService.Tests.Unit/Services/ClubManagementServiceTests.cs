using MatchPoint.Api.Shared.Exceptions;
using MatchPoint.Api.Shared.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Services;
using MatchPoint.ClubService.Tests.Unit.Helpers;
using Moq;

namespace MatchPoint.ClubService.Tests.Unit.Services
{
    [TestClass]
    public class ClubManagementServiceTests
    {
        private Mock<IClubRepository> _clubRepositoryMock = default!;
        private ClubEntityBuilder _clubEntityBuilder = default!;

        [TestInitialize]
        public void Setup()
        {
            _clubRepositoryMock = new Mock<IClubRepository>();
            _clubEntityBuilder = new();
        }

        #region PatchAsync

        [TestMethod]
        public async Task PatchAsync_WhenParametersAreValid_ShouldUpdateOnlySelectedAndTrackingProperties()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            string editedName = "This is an edited club";
            string editedEmail = "edited@email.com";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubEntity.Name), editedName),
                new PropertyUpdate(nameof(clubEntity.Email), editedEmail)
            ];

            _clubRepositoryMock.Setup(s => s.GetByIdAsync(clubEntity.Id, It.IsAny<bool>())).ReturnsAsync(clubEntity);
            _clubRepositoryMock.Setup(s => s.UpdateAsync(It.IsAny<ClubEntity>()));

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            var result = await clubService.PatchAsync(clubEntity.Id, updates);
            #endregion

            #region Assert
            _clubRepositoryMock.Verify(s => s.UpdateAsync(
                It.Is<ClubEntity>(
                    club => club.Name == editedName
                    && club.Email == editedEmail
                    && club.ModifiedBy != default
                    && club.ModifiedOnUtc != default)), Times.Once);
            #endregion
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubIsNotFound_ShouldThrowEntityNotFoundException()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();
            List<PropertyUpdate> updates = [new PropertyUpdate(nameof(ClubEntity.Name), "Integration Testing Club")];
            EntityNotFoundException exception = new("Entity is not found");

            _clubRepositoryMock.Setup(s => s.GetByIdAsync(clubId, It.IsAny<bool>())).Throws(exception);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => clubService.PatchAsync(clubId, updates));
            #endregion
        }

        [TestMethod]
        public async Task PatchAsync_WhenListIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();
            List<PropertyUpdate> updates = null!;

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => clubService.PatchAsync(clubId, updates));
            #endregion
        }

        #endregion
    }
}
