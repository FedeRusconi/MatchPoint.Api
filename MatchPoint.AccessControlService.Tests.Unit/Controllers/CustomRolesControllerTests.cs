using Asp.Versioning;
using MatchPoint.AccessControlService.Controllers;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.AccessControlService.Tests.Unit.Helpers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.AccessControlService.Tests.Unit.Controllers
{
    [TestClass]
    public class CustomRolesControllerTests
    {
        private Mock<ICustomRoleService> _customRoleServiceMock = default!;
        private Mock<ILogger<CustomRolesController>> _loggerMock = default!;
        private CustomRolesController _controller = default!;
        private CustomRoleEntityBuilder _entityBuilder = default!;
        private CustomRoleBuilder _dtoBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _customRoleServiceMock = new();
            _loggerMock = new();
            _controller = new CustomRolesController(_customRoleServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            // Set up API version in HttpContext
            var apiVersionFeature = new Mock<IApiVersioningFeature>();
            apiVersionFeature
                .Setup(f => f.RequestedApiVersion)
                .Returns(new ApiVersion(AccessControlServiceEndpoints.CurrentVersion));
            _controller.ControllerContext.HttpContext.Features.Set(apiVersionFeature.Object);

            _entityBuilder = new CustomRoleEntityBuilder();
            _dtoBuilder = new CustomRoleBuilder();
            _cancellationToken = new CancellationToken();
        }

        #region GetCustomRolesAsync

        [TestMethod]
        public async Task GetCustomRolesAsync_SuccessScenario_ShouldReturnConvertedResponse()
        {
            // Arrange
            List<CustomRoleEntity> serviceResponse = [_entityBuilder.Build()];
            _customRoleServiceMock
                .Setup(s => s.GetAllAsync(_cancellationToken))
                .ReturnsAsync(ServiceResult<IEnumerable<CustomRoleEntity>>.Success(serviceResponse))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetCustomRolesAsync(_cancellationToken);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var response = result.Value;
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType<IEnumerable<CustomRole>>(response);
        }

        [TestMethod]
        public async Task GetCustomRolesAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _customRoleServiceMock
                .Setup(s => s.GetAllAsync(_cancellationToken))
                .ReturnsAsync(ServiceResult<IEnumerable<CustomRoleEntity>>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetCustomRolesAsync(_cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetCustomRolesAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            _customRoleServiceMock
                .Setup(s => s.GetAllAsync(_cancellationToken))
                .ReturnsAsync(ServiceResult<IEnumerable<CustomRoleEntity>>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetCustomRolesAsync(_cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region GetCustomRoleAsync

        [TestMethod]
        public async Task GetCustomRoleAsync_SuccessScenario_ShouldReturnCustomRoleDto()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            _customRoleServiceMock
                .Setup(s => s.GetByIdAsync(customRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(_entityBuilder.WithId(customRoleId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetCustomRoleAsync(customRoleId, _cancellationToken);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var customRole = result.Value;
            Assert.IsNotNull(customRole);
            Assert.AreEqual(customRoleId, customRole.Id);
            Assert.IsInstanceOfType<CustomRole>(customRole);
        }

        [TestMethod]
        public async Task GetCustomRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _customRoleServiceMock
                .Setup(s => s.GetByIdAsync(customRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetCustomRoleAsync(customRoleId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetCustomRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            _customRoleServiceMock
                .Setup(s => s.GetByIdAsync(customRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetCustomRoleAsync(customRoleId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PostCustomRoleAsync

        [TestMethod]
        public async Task PostCustomRoleAsync_SuccessScenario_ShouldReturnCustomRoleDto()
        {
            // Arrange
            CustomRole customRole = _dtoBuilder.Build();
            _customRoleServiceMock
                .Setup(s => s.CreateAsync(It.Is<CustomRoleEntity>(e => e.Id == customRole.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(customRole.ToCustomRoleEntity()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostCustomRoleAsync(customRole, _cancellationToken);
            var x = result.GetType();

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var customRoleResponse = ((ObjectResult)result.Result!).Value as CustomRole;
            Assert.IsNotNull(customRoleResponse);
            Assert.AreEqual(customRole.Id, customRoleResponse.Id);
            Assert.IsInstanceOfType<CustomRole>(customRoleResponse);
        }

        [TestMethod]
        public async Task PostCustomRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            CustomRole customRole = _dtoBuilder.Build();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _customRoleServiceMock
                .Setup(s => s.CreateAsync(It.Is<CustomRoleEntity>(e => e.Id == customRole.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostCustomRoleAsync(customRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PostCustomRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            CustomRole customRole = _dtoBuilder.Build();
            _customRoleServiceMock
                .Setup(s => s.CreateAsync(It.Is<CustomRoleEntity>(e => e.Id == customRole.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostCustomRoleAsync(customRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PutCustomRoleAsync

        [TestMethod]
        public async Task PutCustomRoleAsync_SuccessScenario_ShouldReturnCustomRoleDto()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            CustomRole customRole = _dtoBuilder.WithId(customRoleId).Build();
            _customRoleServiceMock
                .Setup(s => s.UpdateAsync(It.Is<CustomRoleEntity>(e => e.Id == customRoleId), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(customRole.ToCustomRoleEntity()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutCustomRoleAsync(customRoleId, customRole, _cancellationToken);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var customRoleResponse = result.Value;
            Assert.IsNotNull(customRoleResponse);
            Assert.AreEqual(customRoleId, customRoleResponse.Id);
            Assert.IsInstanceOfType<CustomRole>(customRoleResponse);
        }

        [TestMethod]
        public async Task PutCustomRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            CustomRole customRole = _dtoBuilder.WithId(customRoleId).Build();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _customRoleServiceMock
                .Setup(s => s.UpdateAsync(It.Is<CustomRoleEntity>(e => e.Id == customRoleId), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutCustomRoleAsync(customRoleId, customRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PutCustomRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            CustomRole customRole = _dtoBuilder.WithId(customRoleId).Build();
            _customRoleServiceMock
                .Setup(s => s.UpdateAsync(It.Is<CustomRoleEntity>(e => e.Id == customRoleId), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutCustomRoleAsync(customRoleId, customRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PatchCustomRoleAsync

        [TestMethod]
        public async Task PatchCustomRoleAsync_SuccessScenario_ShouldReturnCustomRoleDto()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            CustomRole customRole = _dtoBuilder.WithId(customRoleId).Build();
            string updatedName = "Updated Custom Role Name";
            ActiveStatus updatedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> propertyUpdates =
            [
                new() { Property = nameof(CustomRole.Name), Value = updatedName },
                new() { Property = nameof(CustomRole.ActiveStatus), Value = updatedStatus },
            ];
            CustomRoleEntity updatedEntity = customRole.ToCustomRoleEntity();
            updatedEntity.Name = updatedName;
            updatedEntity.ActiveStatus = updatedStatus;

            _customRoleServiceMock
                .Setup(s => s.PatchAsync(customRoleId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(updatedEntity))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchCustomRoleAsync(customRoleId, propertyUpdates, _cancellationToken);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var customRoleResponse = result.Value;
            Assert.IsNotNull(customRoleResponse);
            Assert.AreEqual(customRoleId, customRoleResponse.Id);
            Assert.AreEqual(updatedName, customRoleResponse.Name);
            Assert.AreEqual(updatedStatus, customRoleResponse.ActiveStatus);
            Assert.IsInstanceOfType<CustomRole>(customRoleResponse);
        }

        [TestMethod]
        public async Task PatchCustomRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _customRoleServiceMock
                .Setup(s => s.PatchAsync(customRoleId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchCustomRoleAsync(customRoleId, [], _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PatchCustomRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();

            _customRoleServiceMock
                .Setup(s => s.PatchAsync(customRoleId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchCustomRoleAsync(customRoleId, [], _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region DeleteCustomRoleAsync

        [TestMethod]
        public async Task DeleteCustomRoleAsync_SuccessScenario_ShouldReturnNoContent()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();

            _customRoleServiceMock
                .Setup(s => s.DeleteAsync(customRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(_entityBuilder.WithId(customRoleId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteCustomRoleAsync(customRoleId, _cancellationToken);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<NoContentResult>(result);
            Assert.AreEqual(204, ((NoContentResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteCustomRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _customRoleServiceMock
                .Setup(s => s.DeleteAsync(customRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteCustomRoleAsync(customRoleId, _cancellationToken);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteCustomRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid customRoleId = Guid.NewGuid();

            _customRoleServiceMock
                .Setup(s => s.DeleteAsync(customRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<CustomRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteCustomRoleAsync(customRoleId, _cancellationToken);

            // Assert
            _customRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<ObjectResult>(result);
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        #endregion
    }
}
