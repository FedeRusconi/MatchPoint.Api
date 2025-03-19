using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.AccessControlService.Services;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.ServiceDefaults;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.AccessControlService.Tests.Unit.Services
{
    [TestClass]
    public class ClubRoleServiceTests
    {
        private Mock<IClubRoleRepository> _clubRoleRepositoryMock = default!;
        private Mock<ISessionService> _sessionServiceServiceMock = default!;
        private Mock<ILogger<ClubRoleService>> _loggerMock = default!;
        private ClubRoleEntityBuilder _clubRoleEntityBuilder = default!;
        private ClubRoleService _clubRoleService = default!;
        private readonly Guid _clubId = Guid.NewGuid();
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _clubRoleRepositoryMock = new();
            _sessionServiceServiceMock = new();
            _loggerMock = new();
            _clubRoleEntityBuilder = new();
            _clubRoleService = new(_clubRoleRepositoryMock.Object, _sessionServiceServiceMock.Object, _loggerMock.Object);
            _cancellationToken = new CancellationToken();

            _sessionServiceServiceMock.SetupGet(s => s.CurrentUserId).Returns(Guid.NewGuid());
        }

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.GetByIdAsync(_clubId, clubRoleEntity.Id, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnFailResult()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleId, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync((ClubRoleEntity?)null);

            // Act
            var result = await _clubRoleService.GetByIdAsync(_clubId, clubRoleId, _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            // Assign a different club id for the test
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(Guid.NewGuid()).Build();

            _clubRoleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.GetByIdAsync(_clubId, clubRoleEntity.Id, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsProvided_ShouldReturnSuccessResult()
        {
            // Arrange
            int pageNumber = 3;
            int pageSize = 10;
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();
            PagedResponse<ClubRoleEntity> expectedResponse = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = 40,
                Data = [clubRoleEntity]
            };

            _clubRoleRepositoryMock
                .Setup(repo => repo.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, _cancellationToken, It.IsAny<Dictionary<string, string>>(), null, false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.GetAllWithSpecificationAsync(
                _clubId, pageNumber, pageSize, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreProvided_ShouldCallRepoMethodWithFiltersAndReturnSuccessResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();
            Dictionary<string, string> filters = new()
            {
                { nameof(ClubRoleEntity.Name), "Test" },
                { nameof(ClubRoleEntity.ActiveStatus), ActiveStatus.Active.ToString() }
            };
            PagedResponse<ClubRoleEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubRoleEntity]
            };

            _clubRoleRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    expectedResponse.CurrentPage,
                    expectedResponse.PageSize,
                    _cancellationToken,
                    filters,
                    null,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, filters: filters, cancellationToken: _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsProvided_ShouldCallRepoMethodWithSortingAndReturnSuccessResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();
            Dictionary<string, SortDirection> orderBy = new()
            {
                { nameof(ClubRoleEntity.Name), SortDirection.Descending }
            };
            PagedResponse<ClubRoleEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [clubRoleEntity]
            };

            _clubRoleRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    _cancellationToken,
                    It.IsAny<Dictionary<string, string>>(),
                    orderBy,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, orderBy: orderBy, cancellationToken: _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        [DataRow(0, 10)]
        [DataRow(1, 0)]
        [DataRow(1, Constants.MaxPageSizeAllowed + 1)]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsInvalid_ShouldReturnFailResult(int pageNumber, int pageSize)
        {
            // Act
            var result = await _clubRoleService.GetAllWithSpecificationAsync(
                _clubId, pageNumber, pageSize, _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreInvalid_ShouldReturnFailResult()
        {
            // Arrange
            Dictionary<string, string> filters = new()
            {
                { "NonExistentProperty", "Test" }
            };

            // Act
            var result = await _clubRoleService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, filters: filters, cancellationToken: _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsInvalid_ShouldReturnFailResult()
        {
            // Arrange=
            Dictionary<string, SortDirection> orderBy = new()
            {
                { "NonExistentProperty", SortDirection.Descending }
            };

            // Act
            var result = await _clubRoleService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, orderBy: orderBy, cancellationToken: _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenClubRoleIsValid_ShouldSetTrackingCreateAndReturnSuccessResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubRoleEntity.Name), clubRoleEntity.Name }
            };

            _clubRoleRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _clubRoleRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ClubRoleEntity>(), _cancellationToken))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.CreateAsync(_clubId, clubRoleEntity, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubRoleEntity.Name, result.Data.Name);
            Assert.AreEqual(_clubId, result.Data.ClubId);
            Assert.AreNotEqual(default, result.Data.CreatedBy);
            Assert.AreNotEqual(default, result.Data.CreatedOnUtc);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubRoleIdIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.CreateAsync(clubRoleEntity, _cancellationToken))
                .ReturnsAsync((ClubRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.CreateAsync(_clubId, clubRoleEntity, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubRoleNameIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(ClubRoleEntity.Name), clubRoleEntity.Name }
            };

            _clubRoleRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.CreateAsync(_clubId, clubRoleEntity, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenClubRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRoleService.CreateAsync(_clubId, clubRoleEntity, _cancellationToken));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenClubRoleIsValid_ShouldSetTrackingUpdateAndReturnSuccessResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.UpdateAsync(clubRoleEntity, _cancellationToken))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.UpdateAsync(_clubId, clubRoleEntity, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubRoleEntity.Name, result.Data.Name);
            Assert.AreEqual(_clubId, result.Data.ClubId);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubRoleIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.UpdateAsync(clubRoleEntity, _cancellationToken))
                .ReturnsAsync((ClubRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.UpdateAsync(_clubId, clubRoleEntity, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenClubRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRoleService.UpdateAsync(_clubId, clubRoleEntity, _cancellationToken));
        }

        #endregion

        #region PatchAsync

        [TestMethod]
        public async Task PatchAsync_WhenParametersAreValid_ShouldUpdateOnlySelectedAndTrackingPropertiesAndReturnSuccessResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();
            string editedName = "This is an edited club role";
            var editedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubRoleEntity.Name), editedName),
                new PropertyUpdate(nameof(clubRoleEntity.ActiveStatus), editedStatus)
            ];

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);
            _clubRoleRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<ClubRoleEntity>(), _cancellationToken))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _clubRoleService.PatchAsync(_clubId, clubRoleEntity.Id, updates, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(editedName, result.Data.Name);
            Assert.AreEqual(editedStatus, result.Data.ActiveStatus);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubRoleIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            List<PropertyUpdate> updates = [new PropertyUpdate(nameof(ClubRoleEntity.Name), "Integration Testing Club Role")];

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleId, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync((ClubRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.PatchAsync(_clubId, clubRoleId, updates, _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenPropertyUpdatesAreInvalid_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();
            List<PropertyUpdate> updates = [
                new PropertyUpdate("Invalid Property", "Value")
            ];

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.PatchAsync(_clubId, clubRoleEntity.Id, updates, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenRepoReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();
            string editedName = "This is an edited club role";
            var editedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubRoleEntity.Name), editedName),
                new PropertyUpdate(nameof(clubRoleEntity.ActiveStatus), editedStatus)
            ];

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);
            _clubRoleRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<ClubRoleEntity>(), _cancellationToken))
                .ReturnsAsync((ClubRoleEntity?)null)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _clubRoleService.PatchAsync(_clubId, clubRoleEntity.Id, updates, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(Guid.NewGuid()).Build();
            string editedName = "This is an edited club role";
            var editedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(clubRoleEntity.Name), editedName),
                new PropertyUpdate(nameof(clubRoleEntity.ActiveStatus), editedStatus)
            ];

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, false))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.PatchAsync(
                _clubId, clubRoleEntity.Id, updates, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenListIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            List<PropertyUpdate> updates = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _clubRoleService.PatchAsync(_clubId, clubRoleId, updates, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, true))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);
            _clubRoleRepositoryMock.Setup(repo => repo.DeleteAsync(clubRoleEntity, _cancellationToken))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.DeleteAsync(_clubId, clubRoleEntity.Id, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(clubRoleEntity.Id, result.Data.Id);
            Assert.AreEqual(clubRoleEntity.Name, result.Data.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubRoleIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, true))
                .ReturnsAsync((ClubRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.DeleteAsync(_clubId, clubRoleEntity.Id, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenDeleteReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(_clubId).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, true))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);
            _clubRoleRepositoryMock.Setup(repo => repo.DeleteAsync(clubRoleEntity, _cancellationToken))
                .ReturnsAsync((ClubRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.DeleteAsync(_clubId, clubRoleEntity.Id, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            var clubRoleEntity = _clubRoleEntityBuilder.WithClubId(Guid.NewGuid()).Build();

            _clubRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(clubRoleEntity.Id, _cancellationToken, true))
                .ReturnsAsync(clubRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _clubRoleService.DeleteAsync(_clubId, clubRoleEntity.Id, _cancellationToken);

            // Assert
            _clubRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion
    }
}
