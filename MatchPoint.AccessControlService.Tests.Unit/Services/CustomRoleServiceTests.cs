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
    public class CustomRoleServiceTests
    {
        private Mock<ICustomRoleRepository> _customRoleRepositoryMock = default!;
        private Mock<ISessionService> _sessionServiceServiceMock = default!;
        private Mock<ILogger<CustomRoleService>> _loggerMock = default!;
        private CustomRoleEntityBuilder _customRoleEntityBuilder = default!;
        private CustomRoleService _customRoleService = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _customRoleRepositoryMock = new();
            _sessionServiceServiceMock = new();
            _loggerMock = new();
            _customRoleEntityBuilder = new();
            _customRoleService = new(_customRoleRepositoryMock.Object, _sessionServiceServiceMock.Object, _loggerMock.Object);
            _cancellationToken = new CancellationToken();

            _sessionServiceServiceMock.SetupGet(s => s.CurrentUserId).Returns(Guid.NewGuid());
        }

        #region GetByIdAsync

        [TestMethod]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.GetByIdAsync(customRoleEntity.Id, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnFailResult()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleId, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync((CustomRoleEntity?)null);

            // Act
            var result = await _customRoleService.GetByIdAsync(customRoleId, _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        #endregion

        #region GetAllWithSpecificationAsync

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenPagingIsProvided_ShouldReturnSuccessResult()
        {
            // Arrange
            int pageNumber = 3;
            int pageSize = 10;
            var customRoleEntity = _customRoleEntityBuilder.Build();
            PagedResponse<CustomRoleEntity> expectedResponse = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = 40,
                Data = [customRoleEntity]
            };

            _customRoleRepositoryMock
                .Setup(repo => repo.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, _cancellationToken, It.IsAny<Dictionary<string, string>>(), null, false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.GetAllWithSpecificationAsync(pageNumber, pageSize, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenFiltersAreProvided_ShouldCallRepoMethodWithFiltersAndReturnSuccessResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();
            Dictionary<string, string> filters = new()
            {
                { nameof(CustomRoleEntity.Name), "Test" },
                { nameof(CustomRoleEntity.ActiveStatus), ActiveStatus.Active.ToString() }
            };
            PagedResponse<CustomRoleEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [customRoleEntity]
            };

            _customRoleRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    expectedResponse.CurrentPage,
                    expectedResponse.PageSize,
                    _cancellationToken,
                    filters,
                    null,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.GetAllWithSpecificationAsync(
                1, 500, filters: filters, cancellationToken: _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreNotEqual(0, result.Data.Data.Count());
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetAllWithSpecificationAsync_WhenSortingIsProvided_ShouldCallRepoMethodWithSortingAndReturnSuccessResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();
            Dictionary<string, SortDirection> orderBy = new()
            {
                { nameof(CustomRoleEntity.Name), SortDirection.Descending }
            };
            PagedResponse<CustomRoleEntity> expectedResponse = new()
            {
                CurrentPage = 1,
                PageSize = 500,
                TotalCount = 40,
                Data = [customRoleEntity]
            };

            _customRoleRepositoryMock.Setup(repo => repo.GetAllWithSpecificationAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    _cancellationToken,
                    It.IsAny<Dictionary<string, string>>(),
                    orderBy,
                    false))
                .ReturnsAsync(expectedResponse)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.GetAllWithSpecificationAsync(
                1, 500, orderBy: orderBy, cancellationToken: _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
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
            var result = await _customRoleService.GetAllWithSpecificationAsync(
                pageNumber, pageSize, _cancellationToken);

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
            var result = await _customRoleService.GetAllWithSpecificationAsync(
                1, 500, filters: filters, cancellationToken: _cancellationToken);

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
            var result = await _customRoleService.GetAllWithSpecificationAsync(
                1, 500, orderBy: orderBy, cancellationToken: _cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        #endregion

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_WhenCustomRoleIsValid_ShouldSetTrackingCreateAndReturnSuccessResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(CustomRoleEntity.Name), customRoleEntity.Name }
            };

            _customRoleRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(0)
                .Verifiable(Times.Once);
            _customRoleRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<CustomRoleEntity>(), _cancellationToken))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.CreateAsync(customRoleEntity, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(customRoleEntity.Name, result.Data.Name);
            Assert.AreNotEqual(default, result.Data.CreatedBy);
            Assert.AreNotEqual(default, result.Data.CreatedOnUtc);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCustomRoleIdIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            _customRoleRepositoryMock.Setup(repo => repo.CreateAsync(customRoleEntity, _cancellationToken))
                .ReturnsAsync((CustomRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.CreateAsync(customRoleEntity, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCustomRoleNameIsDuplicate_ShouldReturnFailResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();
            var countFilters = new Dictionary<string, string>()
            {
                { nameof(CustomRoleEntity.Name), customRoleEntity.Name }
            };

            _customRoleRepositoryMock.Setup(repo => repo.CountAsync(_cancellationToken, countFilters))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.CreateAsync(customRoleEntity, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.Conflict, result.ResultType);
        }

        [TestMethod]
        public async Task CreateAsync_WhenCustomRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _customRoleService.CreateAsync(customRoleEntity, _cancellationToken));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_WhenCustomRoleIsValid_ShouldSetTrackingUpdateAndReturnSuccessResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            _customRoleRepositoryMock.Setup(repo => repo.UpdateAsync(customRoleEntity, _cancellationToken))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.UpdateAsync(customRoleEntity, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(customRoleEntity.Name, result.Data.Name);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenCustomRoleIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            _customRoleRepositoryMock.Setup(repo => repo.UpdateAsync(customRoleEntity, _cancellationToken))
                .ReturnsAsync((CustomRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.UpdateAsync(customRoleEntity, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenCustomRoleIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            CustomRoleEntity customRoleEntity = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _customRoleService.UpdateAsync(customRoleEntity, _cancellationToken));
        }

        #endregion

        #region PatchAsync

        [TestMethod]
        public async Task PatchAsync_WhenParametersAreValid_ShouldUpdateOnlySelectedAndTrackingPropertiesAndReturnSuccessResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();
            string editedName = "This is an edited custom role";
            var editedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(customRoleEntity.Name), editedName),
                new PropertyUpdate(nameof(customRoleEntity.ActiveStatus), editedStatus)
            ];

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);
            _customRoleRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<CustomRoleEntity>(), _cancellationToken))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _customRoleService.PatchAsync(customRoleEntity.Id, updates, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(editedName, result.Data.Name);
            Assert.AreEqual(editedStatus, result.Data.ActiveStatus);
            Assert.AreNotEqual(default, result.Data.ModifiedBy);
            Assert.AreNotEqual(default, result.Data.ModifiedOnUtc);
        }

        [TestMethod]
        public async Task PatchAsync_WhenCustomRoleIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            List<PropertyUpdate> updates = [new PropertyUpdate(nameof(CustomRoleEntity.Name), "Integration Testing Custom Role")];

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleId, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync((CustomRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.PatchAsync(customRoleId, updates, _cancellationToken);

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
            var customRoleEntity = _customRoleEntityBuilder.Build();
            List<PropertyUpdate> updates = [
                new PropertyUpdate("Invalid Property", "Value")
            ];

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.PatchAsync(customRoleEntity.Id, updates, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.BadRequest, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenRepoReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();
            string editedName = "This is an edited custom role";
            var editedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> updates = [
                new PropertyUpdate(nameof(customRoleEntity.Name), editedName),
                new PropertyUpdate(nameof(customRoleEntity.ActiveStatus), editedStatus)
            ];

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleEntity.Id, _cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);
            _customRoleRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<CustomRoleEntity>(), _cancellationToken))
                .ReturnsAsync((CustomRoleEntity?)null)
                .Verifiable(Times.Once); ;

            // Act
            var result = await _customRoleService.PatchAsync(customRoleEntity.Id, updates, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task PatchAsync_WhenListIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            List<PropertyUpdate> updates = null!;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _customRoleService.PatchAsync(customRoleId, updates, _cancellationToken));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_WhenIdIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleEntity.Id, _cancellationToken, true))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);
            _customRoleRepositoryMock.Setup(repo => repo.DeleteAsync(customRoleEntity, _cancellationToken))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.DeleteAsync(customRoleEntity.Id, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(customRoleEntity.Id, result.Data.Id);
            Assert.AreEqual(customRoleEntity.Name, result.Data.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenCustomRoleIsNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleEntity.Id, _cancellationToken, true))
                .ReturnsAsync((CustomRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.DeleteAsync(customRoleEntity.Id, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenDeleteReturnsNull_ShouldReturnFailResult()
        {
            // Arrange
            var customRoleEntity = _customRoleEntityBuilder.Build();

            _customRoleRepositoryMock.Setup(repo => repo.GetByIdAsync(customRoleEntity.Id, _cancellationToken, true))
                .ReturnsAsync(customRoleEntity)
                .Verifiable(Times.Once);
            _customRoleRepositoryMock.Setup(repo => repo.DeleteAsync(customRoleEntity, _cancellationToken))
                .ReturnsAsync((CustomRoleEntity?)null)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.DeleteAsync(customRoleEntity.Id, _cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ServiceResultType.NotFound, result.ResultType);
        }

        #endregion
    }
}
