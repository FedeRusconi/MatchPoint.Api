using System.Net;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Services;
using MatchPoint.ServiceDefaults;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.ClubService.Tests.Unit.Services
{
    [TestClass]
    public class CourtServiceTests
    {
        private Mock<ICourtRepository> _courtRepositoryMock = default!;
        private Mock<ISessionService> _sessionServiceMock = default!;
        private Mock<ILogger<CourtService>> _loggerMock = default!;
        private CourtEntityBuilder _courtEntityBuilder = default!;
        private CourtService _courtService = default!;
        private readonly Guid _clubId = Guid.NewGuid();
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _courtRepositoryMock = new();
            _sessionServiceMock = new();
            _loggerMock = new();
            _courtEntityBuilder = new();
            _courtService = new(
                _courtRepositoryMock.Object,
                _sessionServiceMock.Object,
                _loggerMock.Object);
            _cancellationToken = new CancellationToken();

            _sessionServiceMock.SetupGet(s => s.CurrentUserId).Returns(Guid.NewGuid());
        }

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();

            _courtRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.GetByIdAsync(_clubId, courtEntity.Id, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnFailResult()
        {
            // Arrange
            Guid courtId = Guid.NewGuid();

            _courtRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courtId, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync((CourtEntity?)null);

            // Act
            var result = await _courtService.GetByIdAsync(_clubId, courtId, _cancellationToken);

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
            var courtEntity = _courtEntityBuilder.WithClubId(Guid.NewGuid()).Build();

            _courtRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.GetByIdAsync(_clubId, courtEntity.Id, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
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
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            PagedResponse<CourtEntity> expectedResponse = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = 40,
                Data = [courtEntity]
            };

            _courtRepositoryMock
                .Setup(repo => repo.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, _cancellationToken, It.IsAny<Dictionary<string, string>>(), null, false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.GetAllWithSpecificationAsync(
                _clubId, pageNumber, pageSize, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreProvided_ShouldCallRepoMethodWithFiltersAndReturnSuccessResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            Dictionary<string, string> filters = new()
            {
                { nameof(CourtEntity.Name), "Test" },
                { nameof(CourtEntity.ActiveStatus), ActiveStatus.Active.ToString() }
            };
            PagedResponse<CourtEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [courtEntity]
            };

            _courtRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    expectedResponse.CurrentPage,
                    expectedResponse.PageSize,
                    _cancellationToken,
                    filters,
                    null,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, filters: filters);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsProvided_ShouldCallRepoMethodWithSortingAndReturnSuccessResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            Dictionary<string, SortDirection> orderBy = new()
            {
                { nameof(CourtEntity.Name), SortDirection.Descending }
            };
            PagedResponse<CourtEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [courtEntity]
            };

            _courtRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    _cancellationToken,
                    It.IsAny<Dictionary<string, string>>(),
                    orderBy,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, orderBy: orderBy);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        [DataRow(0, 10)]
        [DataRow(1, 0)]
        [DataRow(1, Api.Shared.Common.Utilities.Constants.MaxPageSizeAllowed + 1)]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsInvalid_ShouldReturnFailResult(int pageNumber, int pageSize)
        {
            // Act
            var result = await _courtService.GetAllWithSpecificationAsync(
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
            var result = await _courtService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, filters: filters);

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
            var result = await _courtService.GetAllWithSpecificationAsync(
                _clubId, 1, 500, _cancellationToken, orderBy: orderBy);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenCourtIsValid_ShouldSetTrackingCreateAndReturnSuccessResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(CourtEntity.Name), courtEntity.Name },
                { nameof(CourtEntity.ClubId), courtEntity.ClubId.ToString() }
            };

            _courtRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _courtRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<CourtEntity>(), _cancellationToken))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.CreateAsync(_clubId, courtEntity, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(courtEntity.Name, result.Data.Name);
            Assert.AreEqual(_clubId, result.Data.ClubId);
            Assert.AreNotEqual(default, result.Data.CreatedBy);
            Assert.AreNotEqual(default, result.Data.CreatedOnUtc);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCourtNameIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(CourtEntity.Name), courtEntity.Name },
                { nameof(CourtEntity.ClubId), courtEntity.ClubId.ToString() }
            };

            _courtRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.CreateAsync(_clubId, courtEntity, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenRepoReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(CourtEntity.Name), courtEntity.Name },
                { nameof(CourtEntity.ClubId), courtEntity.ClubId.ToString() }
            };

            _courtRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _courtRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<CourtEntity>(), _cancellationToken))
                .ReturnsAsync((CourtEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.CreateAsync(_clubId, courtEntity, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCourtIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CourtEntity courtEntity = null!;

            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(
                () => _courtService.CreateAsync(_clubId, courtEntity, _cancellationToken));
        }

        #endregion

        #region PatchAsync

        [TestMethod]
        public async Task PatchAsync_WhenParametersAreValid_ShouldUpdateOnlySelectedAndTrackingPropertiesAndReturnSuccessResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            string editedName = "Name Edited";
            string editedDescription = "Updated Text";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(courtEntity.Name), editedName),
                new PropertyUpdate(nameof(courtEntity.Description), editedDescription)
            ];

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);
            _courtRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<CourtEntity>(), _cancellationToken))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _courtService.PatchAsync(_clubId, courtEntity.Id, updates, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(editedName, result.Data.Name);
            Assert.AreEqual(editedDescription, result.Data.Description);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
        }

        [TestMethod]
        public async Task PatchAsync_WhenCourtIsNotFoundInDb_ShouldReturnFailResult()
        {
            // Arrange
            Guid courtId = Guid.NewGuid();
            List<PropertyUpdate> updates = [new PropertyUpdate(nameof(CourtEntity.Name), "Test")];

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtId, _cancellationToken, false))
                .ReturnsAsync((CourtEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.PatchAsync(_clubId, courtId, updates, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenPropertyUpdatesAreInvalid_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.Build();
            List<PropertyUpdate> updates = [
                new PropertyUpdate("Invalid Property", "Value")
            ];

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.PatchAsync(_clubId, courtEntity.Id, updates, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenRepoReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            string editedName = "Name Edited";
            string editedDescription = "Updated Text";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(courtEntity.Name), editedName),
                new PropertyUpdate(nameof(courtEntity.Description), editedDescription)
            ];

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);
            _courtRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<CourtEntity>(), _cancellationToken))
                .ReturnsAsync((CourtEntity?)null)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _courtService.PatchAsync(_clubId, courtEntity.Id, updates, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(Guid.NewGuid()).Build();
            string editedName = "Name Edited";
            string editedDescription = "Updated Text";
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(courtEntity.Name), editedName),
                new PropertyUpdate(nameof(courtEntity.Description), editedDescription)
            ];

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.PatchAsync(
                _clubId, courtEntity.Id, updates, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenListIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Guid courtId = Guid.NewGuid();
            List<PropertyUpdate> updates = null!;

            // Act & Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(
                () => _courtService.PatchAsync(
                    _clubId, courtId, updates, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);
            _courtRepositoryMock.Setup(repo => repo.DeleteAsync(courtEntity, _cancellationToken))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.DeleteAsync(_clubId, courtEntity.Id, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(courtEntity.Id, result.Data.Id);
            Assert.AreEqual(courtEntity.Name, result.Data.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenCourtIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync((CourtEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.DeleteAsync(_clubId, courtEntity.Id, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenRepositoryOperationFails_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(_clubId).Build();
            var exception = new HttpRequestException("This is a test error", null, HttpStatusCode.Conflict);

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);
            _courtRepositoryMock.Setup(repo => repo.DeleteAsync(courtEntity, _cancellationToken))
                .ReturnsAsync((CourtEntity)null!)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.DeleteAsync(_clubId, courtEntity.Id, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenClubIdMismatch_ShouldReturnFailResult()
        {
            // Arrange
            var courtEntity = _courtEntityBuilder.WithClubId(Guid.NewGuid()).Build();

            _courtRepositoryMock.Setup(repo => repo.GetByIdAsync(courtEntity.Id, _cancellationToken, false))
                .ReturnsAsync(courtEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _courtService.DeleteAsync(_clubId, courtEntity.Id, _cancellationToken);

            // Assert
            _courtRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion
    }
}
