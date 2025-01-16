using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Services;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.ServiceDefaults;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.AccessControlService.Tests.Unit.Services
{
    public class CustomRoleServiceTests
    {
        private Mock<IRepository<CustomRoleEntity>> _customRoleRepositoryMock = default!;
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

        #region GetAllAsync

        [TestMethod]
        public async Task GetAllAsync_WhenRequestIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            var customRoleEntityBuilder2 = new CustomRoleEntityBuilder();
            List<CustomRoleEntity> customRoleEntities = [
                _customRoleEntityBuilder.Build(),
                customRoleEntityBuilder2.Build()
            ];

            _customRoleRepositoryMock.Setup(repo => repo.GetAllAsync(_cancellationToken, It.IsAny<bool>()))
                .ReturnsAsync(customRoleEntities)
                .Verifiable(Times.Once);

            // Act
            var result = await _customRoleService.GetAllAsync(_cancellationToken);

            // Assert
            _customRoleRepositoryMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
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
