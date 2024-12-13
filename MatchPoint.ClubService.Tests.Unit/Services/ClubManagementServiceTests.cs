using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.ClubService.Tests.Unit.Services
{
    [TestClass]
    public class ClubManagementServiceTests
    {
        private Mock<IClubRepository> _clubRepositoryMock = default!;
        private Mock<ILogger<ClubManagementService>> _loggerMock = default!;
        private ClubEntityBuilder _clubEntityBuilder = default!;
        private ClubManagementService _clubService = default!;

        [TestInitialize]
        public void Setup()
        {
            _clubRepositoryMock = new();
            _loggerMock = new();
            _clubEntityBuilder = new();
            _clubService = new(_clubRepositoryMock.Object, _loggerMock.Object);
        }

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
            .Build();

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubEntity.Id, It.IsAny<bool>()))
                .ReturnsAsync(clubEntity);
            #endregion

            #region Act
            var result = await _clubService.GetByIdAsync(clubEntity.Id);
            #endregion

            #region Assert
            _clubRepositoryMock.Verify(repo => repo.GetByIdAsync(clubEntity.Id, false), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            #endregion
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnFailResult()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubId, It.IsAny<bool>())).ReturnsAsync((ClubEntity?)null);
            #endregion

            #region Act
            var result = await _clubService.GetByIdAsync(clubId);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
            #endregion
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsProvided_ShouldReturnSuccessResult()
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
            #endregion

            #region Act
            var result = await _clubService.GetAllWithSpecificationAsync(pageNumber, pageSize);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
            #endregion
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreProvided_ShouldCallRepoMethodWithFiltersAndReturnSuccessResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            Dictionary<string, object> filters = new()
            {
                { nameof(ClubEntity.Name), "Test" },
                { nameof(ClubEntity.Email), 0 },
                { nameof(ClubEntity.ActiveStatus), true },
                { nameof(ClubEntity.Address), DateTime.UtcNow },
            };
            PagedResponse<ClubEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubEntity]
            };

            _clubRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    expectedResponse.CurrentPage,
                    expectedResponse.PageSize,
                    filters,
                    null,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);
            #endregion

            #region Act
            var result = await _clubService.GetAllWithSpecificationAsync(1, 500, filters: filters);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
            #endregion
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsProvided_ShouldCallRepoMethodWithSortingAndReturnSuccessResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder
                .WithName("Integration Testing Club")
                .Build();
            Dictionary<string, SortDirection> orderBy = new()
            {
                { nameof(ClubEntity.Name), SortDirection.Descending },
                { nameof(ClubEntity.CreatedOnUtc), SortDirection.Ascending }
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
                    null,
                    orderBy,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);
            #endregion

            #region Act
            var result = await _clubService.GetAllWithSpecificationAsync(1, 500, orderBy: orderBy);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
            #endregion
        }

        [TestMethod]
        [DataRow(0, 10)]
        [DataRow(1, 0)]
        [DataRow(1, Constants.MaxPageSizeAllowed + 1)]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsInvalid_ShouldReturnFailResult(int pageNumber, int pageSize)
        {
            #region Act
            var result = await _clubService.GetAllWithSpecificationAsync(pageNumber, pageSize);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreInvalid_ShouldReturnFailResult()
        {
            #region Arrange
            Dictionary<string, object> filters = new()
            {
                { "NonExistentProperty", "Test" }
            };
            #endregion

            #region Act
            var result = await _clubService.GetAllWithSpecificationAsync(1, 500, filters: filters);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsInvalid_ShouldReturnFailResult()
        {
            #region Arrange=
            Dictionary<string, SortDirection> orderBy = new()
            {
                { "NonExistentProperty", SortDirection.Descending }
            };
            #endregion

            #region Act
            var result = await _clubService.GetAllWithSpecificationAsync(1, 500, orderBy: orderBy);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
            #endregion
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubIsValid_ShouldSetTrackingCreateAndReturnSuccessResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            var countFilters = new Dictionary<string, object>()
            {
                { nameof(ClubEntity.Email), clubEntity.Email }
            };

            _clubRepositoryMock.Setup(repo => repo.CountAsync(countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _clubRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubEntity>()))
                .ReturnsAsync(clubEntity)
                .Verifiable(Times.Once);

            #endregion

            #region Act
            var result = await _clubService.CreateAsync(clubEntity);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubEntity.Email, result.Data.Email);
            Assert.AreNotEqual(default, result.Data.CreatedBy);
            Assert.AreNotEqual(default, result.Data.CreatedOnUtc);
            #endregion
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubEmailIsDuplicate_ShouldReturnFailResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            var countFilters = new Dictionary<string, object>()
            {
                { nameof(ClubEntity.Email), clubEntity.Email }
            };

            _clubRepositoryMock.Setup(repo => repo.CountAsync(countFilters))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            #endregion

            #region Act
            var result = await _clubService.CreateAsync(clubEntity);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
            #endregion
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubEntity clubEntity = null!;
            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _clubService.CreateAsync(clubEntity));
            #endregion
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsValid_ShouldSetTrackingUpdateAndReturnSuccessResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            _clubRepositoryMock.Setup(repo => repo.UpdateAsync(clubEntity))
                .ReturnsAsync(clubEntity)
                .Verifiable(Times.Once);

            #endregion

            #region Act
            var result = await _clubService.UpdateAsync(clubEntity);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubEntity.Email, result.Data.Email);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
            #endregion
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNotFound_ShouldReturnFailResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            _clubRepositoryMock.Setup(repo => repo.UpdateAsync(clubEntity))
                .ReturnsAsync((ClubEntity?)null)
                .Verifiable(Times.Once);

            #endregion

            #region Act
            var result = await _clubService.UpdateAsync(clubEntity);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
            #endregion
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            ClubEntity clubEntity = null!;
            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _clubService.UpdateAsync(clubEntity));
            #endregion
        }

        #endregion

        #region PatchAsync

        [TestMethod]
        public async Task PatchAsync_WhenParametersAreValid_ShouldUpdateOnlySelectedAndTrackingPropertiesAndReturnSuccessResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();
            string editedName = "This is an edited club";
            string editedEmail = "edited@email.com";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubEntity.Name), editedName),
                new PropertyUpdate(nameof(clubEntity.Email), editedEmail)
            ];

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubEntity.Id, It.IsAny<bool>()))
                .ReturnsAsync(clubEntity)
                .Verifiable(Times.Once);
            _clubRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<ClubEntity>()))
                .ReturnsAsync(clubEntity)
                .Verifiable(Times.Once); ;
            #endregion

            #region Act
            var result = await _clubService.PatchAsync(clubEntity.Id, updates);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(editedName, result.Data.Name);
            Assert.AreEqual(editedEmail, result.Data.Email);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
            #endregion
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubIsNotFound_ShouldReturnFailResult()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();
            List<PropertyUpdate> updates = [new PropertyUpdate(nameof(ClubEntity.Name), "Integration Testing Club")];

            _clubRepositoryMock.Setup(repo => repo.GetByIdAsync(clubId, It.IsAny<bool>()))
                .ReturnsAsync((ClubEntity?)null)
                .Verifiable(Times.Once);
            #endregion

            #region Act
            var result = await _clubService.PatchAsync(clubId, updates);
            #endregion

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
            #endregion
        }

        [TestMethod]
        public async Task PatchAsync_WhenListIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            Guid clubId = Guid.NewGuid();
            List<PropertyUpdate> updates = null!;
            #endregion

            #region Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _clubService.PatchAsync(clubId, updates));
            #endregion
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenIdIsValid_ShouldReturnFailResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            _clubRepositoryMock.Setup(repo => repo.DeleteAsync(clubEntity.Id))
                .ReturnsAsync(clubEntity)
                .Verifiable(Times.Once);
            #endregion

            #region Act
            var result = await _clubService.DeleteAsync(clubEntity.Id);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubEntity.Id, result.Data.Id);
            Assert.AreEqual(clubEntity.Name, result.Data.Name);
            #endregion
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubIsNotFound_ShouldReturnSuccessResult()
        {
            #region Arrange
            var clubEntity = _clubEntityBuilder.WithName("Integration Testing Club").Build();

            _clubRepositoryMock.Setup(repo => repo.DeleteAsync(clubEntity.Id))
                .ReturnsAsync((ClubEntity?)null)
                .Verifiable(Times.Once);
            #endregion

            #region Act
            var result = await _clubService.DeleteAsync(clubEntity.Id);
            #endregion

            #region Assert
            _clubRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
            #endregion
        }

        #endregion
    }
}
