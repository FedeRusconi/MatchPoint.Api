using MatchPoint.Api.Shared.Enums;
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

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldCallRepoMethod()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
            .Build();

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubEntity.Id, It.IsAny<bool>()))
                .ReturnsAsync(clubEntity);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await clubService.GetByIdAsync(clubEntity.Id);
            #endregion

            #region Assert
            _clubRepositoryMock.Verify(repo => repo.GetByIdAsync(clubEntity.Id, false), Times.Once);
            #endregion
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldThrowthrowEntityNotFoundException()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();
            EntityNotFoundException exception = new("Entity is not found");

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubId, It.IsAny<bool>())).Throws(exception);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => clubService.GetByIdAsync(clubId));
            #endregion
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsProvided_ShouldCallRepoMethodWithPaging()
        {
            #region Arrange
            int pageNumber = 3;
            int pageSize = 10;
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            PagedResponse<ClubEntity> expectedResponse = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = 40,
                Data = [clubEntity]
            };

            _clubRepositoryMock
                .Setup(repo => repo.GetAllWithSpecificationAsync(pageNumber, pageSize, null, null, false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await clubService.GetAllWithSpecificationAsync(pageNumber, pageSize);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            #endregion
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsNotProvided_ShouldCallRepoMethodWithDefaultPaging()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            PagedResponse<ClubEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubEntity]
            };

            _clubRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    1, It.Is<int>(size => size > 0), null, null, false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await clubService.GetAllWithSpecificationAsync();
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            #endregion
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreProvided_ShouldCallRepoMethodWithFilters()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            Dictionary<string, object> filters = new()
            {
                { "TestProperty1", "Test" },
                { "TestProperty2", 0 },
                { "TestProperty3", true },
                { "TestProperty4", DateTime.UtcNow },
            };
            PagedResponse<ClubEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubEntity]
            };

            _clubRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    filters,
                    null,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await clubService.GetAllWithSpecificationAsync(filters: filters);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            #endregion
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsProvided_ShouldCallRepoMethodWithSorting()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            KeyValuePair<string, SortDirection> orderBy = new("TetProperty1", SortDirection.Descending);
            PagedResponse<ClubEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubEntity]
            };

            _clubRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    null,
                    orderBy,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await clubService.GetAllWithSpecificationAsync(orderBy: orderBy);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            #endregion
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubIsValid_ShouldSetTrackingCreateAndReturn()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            var countFilters = new Dictionary<string, object>()
            {
                { nameof(ClubEntity.Email), clubEntity.Email }
            };

            _clubRepositoryMock.Setup(repo => repo.CountAsync(countFilters)).ReturnsAsync(0);
            _clubRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubEntity>()))
                .ReturnsAsync(clubEntity);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);

            #endregion

            #region Act
            await clubService.CreateAsync(clubEntity);
            #endregion

            #region Assert
            _clubRepositoryMock.Verify(repo => repo.CountAsync(countFilters), Times.Once);
            _clubRepositoryMock.Verify(repo => repo.CreateAsync(
                It.Is<ClubEntity>(
                    c => c.Email == clubEntity.Email && c.CreatedBy != default && c.CreatedOnUtc != default)),
                Times.Once);
            #endregion
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubEmailIsDuplicate_ShouldThrowDuplicateEntityException()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            var countFilters = new Dictionary<string, object>()
            {
                { nameof(ClubEntity.Email), clubEntity.Email }
            };

            _clubRepositoryMock.Setup(repo => repo.CountAsync(countFilters)).ReturnsAsync(1);
            _clubRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubEntity>()))
                .ReturnsAsync(clubEntity);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);

            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<DuplicateEntityException>(() => clubService.CreateAsync(clubEntity));

            _clubRepositoryMock.Verify(repo => repo.CountAsync(countFilters), Times.Once);
            _clubRepositoryMock.Verify(repo => repo.CreateAsync(
                It.Is<ClubEntity>(
                    c => c.Email == clubEntity.Email && c.CreatedBy != default && c.CreatedOnUtc != default)),
                Times.Never);
            #endregion
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubEntity clubEntity = null!;
            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => clubService.CreateAsync(clubEntity));
            #endregion
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsValid_ShouldSetTrackingUpdateAndReturn()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            _clubRepositoryMock.Setup(repo => repo.UpdateAsync(clubEntity))
                .ReturnsAsync(clubEntity);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);

            #endregion

            #region Act
            await clubService.UpdateAsync(clubEntity);
            #endregion

            #region Assert
            _clubRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<ClubEntity>(
                    c => c.Email == clubEntity.Email && c.ModifiedBy != default && c.ModifiedOnUtc != default)),
                Times.Once);
            #endregion
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNotFound_ShouldThrowEntityNotFoundException()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            var exception = new EntityNotFoundException("Club was not found");

            _clubRepositoryMock.Setup(repo => repo.UpdateAsync(clubEntity))
                .Throws(exception);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);

            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => clubService.UpdateAsync(clubEntity));

            _clubRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<ClubEntity>(
                    c => c.Email == clubEntity.Email && c.ModifiedBy != default && c.ModifiedOnUtc != default)),
                Times.Once);
            #endregion
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubEntity clubEntity = null!;
            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => clubService.UpdateAsync(clubEntity));
            #endregion
        }

        #endregion

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

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubEntity.Id, It.IsAny<bool>())).ReturnsAsync(clubEntity);
            _clubRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<ClubEntity>()));

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            var result = await clubService.PatchAsync(clubEntity.Id, updates);
            #endregion

            #region Assert
            _clubRepositoryMock.Verify(repo => repo.UpdateAsync(
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

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubId, It.IsAny<bool>())).Throws(exception);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act & Assert
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

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => clubService.PatchAsync(clubId, updates));
            #endregion
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenIdIsValid_ShouldCallRepoMethod()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();

            _clubRepositoryMock.Setup(repo => repo.DeleteAsync(clubId)).ReturnsAsync(true);

            ClubManagementService clubService = new(_clubRepositoryMock.Object);
            #endregion

            #region Act
            await clubService.DeleteAsync(clubId);
            #endregion

            #region Assert
            _clubRepositoryMock.Verify(repo => repo.DeleteAsync(clubId), Times.Once);
            #endregion
        }

        #endregion
    }
}
