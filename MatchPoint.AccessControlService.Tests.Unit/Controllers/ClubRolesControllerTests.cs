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
    public class ClubRolesControllerTests
    {
        private Mock<IClubRoleService> _clubRoleServiceMock = default!;
        private Mock<ILogger<ClubRolesController>> _loggerMock = default!;
        private ClubRolesController _controller = default!;
        private ClubRoleEntityBuilder _entityBuilder = default!;
        private ClubRoleBuilder _dtoBuilder = default!;
        private CancellationToken _cancellationToken = default!;
        private Guid _clubId = Guid.NewGuid();

        [TestInitialize]
        public void TestInitialize()
        {
            _clubRoleServiceMock = new();
            _loggerMock = new();
            _controller = new ClubRolesController(_clubRoleServiceMock.Object, _loggerMock.Object)
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

            _entityBuilder = new ClubRoleEntityBuilder();
            _dtoBuilder = new ClubRoleBuilder();
            _cancellationToken = new CancellationToken();
        }

        #region GetClubRolesAsync

        [TestMethod]
        public async Task GetClubRolesAsync_SuccessScenario_ShouldReturnConvertedResponse()
        {
            // Arrange
            PagedResponse<ClubRoleEntity> serviceResponse = new() { Data = [_entityBuilder.WithClubId(_clubId).Build()] };
            _clubRoleServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(
                    _clubId, 
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    _cancellationToken,
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Dictionary<string, SortDirection>>()))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubRoleEntity>>.Success(serviceResponse))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubRolesAsync(_clubId, _cancellationToken);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var response = result.Value;
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType<PagedResponse<ClubRole>>(response);
        }

        [TestMethod]
        public async Task GetClubRolesAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubRoleServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(
                    _clubId,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    _cancellationToken,
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Dictionary<string, SortDirection>>()))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubRoleEntity>>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubRolesAsync(_clubId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetClubRolesAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            _clubRoleServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(
                    _clubId,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    _cancellationToken,
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Dictionary<string, SortDirection>>()))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubRoleEntity>>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubRolesAsync(_clubId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region GetClubRoleAsync

        [TestMethod]
        public async Task GetClubRoleAsync_SuccessScenario_ShouldReturnClubRoleDto()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            _clubRoleServiceMock
                .Setup(s => s.GetByIdAsync(_clubId, clubRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(_entityBuilder.WithId(clubRoleId).WithClubId(_clubId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubRoleAsync(_clubId, clubRoleId, _cancellationToken);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubRole = result.Value;
            Assert.IsNotNull(clubRole);
            Assert.AreEqual(clubRoleId, clubRole.Id);
            Assert.IsInstanceOfType<ClubRole>(clubRole);
        }

        [TestMethod]
        public async Task GetClubRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubRoleServiceMock
                .Setup(s => s.GetByIdAsync(_clubId, clubRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubRoleAsync(_clubId, clubRoleId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetClubRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            _clubRoleServiceMock
                .Setup(s => s.GetByIdAsync(_clubId, clubRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubRoleAsync(_clubId, clubRoleId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PostClubRoleAsync

        [TestMethod]
        public async Task PostClubRoleAsync_SuccessScenario_ShouldReturnClubRoleDto()
        {
            // Arrange
            ClubRole clubRole = _dtoBuilder.WithClubId(_clubId).Build();
            _clubRoleServiceMock
                .Setup(s => s.CreateAsync(_clubId, It.Is<ClubRoleEntity>(e => e.Id == clubRole.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(clubRole.ToClubRoleEntity()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubRoleAsync(_clubId, clubRole, _cancellationToken);
            var x = result.GetType();

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubRoleResponse = ((ObjectResult)result.Result!).Value as ClubRole;
            Assert.IsNotNull(clubRoleResponse);
            Assert.AreEqual(clubRole.Id, clubRoleResponse.Id);
            Assert.IsInstanceOfType<ClubRole>(clubRoleResponse);
        }

        [TestMethod]
        public async Task PostClubRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            ClubRole clubRole = _dtoBuilder.Build();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubRoleServiceMock
                .Setup(s => s.CreateAsync(_clubId, It.Is<ClubRoleEntity>(e => e.Id == clubRole.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubRoleAsync(_clubId, clubRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PostClubRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            ClubRole clubRole = _dtoBuilder.Build();
            _clubRoleServiceMock
                .Setup(s => s.CreateAsync(_clubId, It.Is<ClubRoleEntity>(e => e.Id == clubRole.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubRoleAsync(_clubId, clubRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PutClubRoleAsync

        [TestMethod]
        public async Task PutClubRoleAsync_SuccessScenario_ShouldReturnClubRoleDto()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            ClubRole clubRole = _dtoBuilder.WithId(clubRoleId).WithClubId(_clubId).Build();
            _clubRoleServiceMock
                .Setup(s => s.UpdateAsync(_clubId, It.Is<ClubRoleEntity>(e => e.Id == clubRoleId), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(clubRole.ToClubRoleEntity()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutClubRoleAsync(_clubId, clubRoleId, clubRole, _cancellationToken);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubRoleResponse = result.Value;
            Assert.IsNotNull(clubRoleResponse);
            Assert.AreEqual(clubRoleId, clubRoleResponse.Id);
            Assert.IsInstanceOfType<ClubRole>(clubRoleResponse);
        }

        [TestMethod]
        public async Task PutClubRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            ClubRole clubRole = _dtoBuilder.WithId(clubRoleId).WithClubId(_clubId).Build();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubRoleServiceMock
                .Setup(s => s.UpdateAsync(_clubId, It.Is<ClubRoleEntity>(e => e.Id == clubRoleId), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutClubRoleAsync(_clubId, clubRoleId, clubRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PutClubRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            ClubRole clubRole = _dtoBuilder.WithId(clubRoleId).WithClubId(_clubId).Build();
            _clubRoleServiceMock
                .Setup(s => s.UpdateAsync(_clubId, It.Is<ClubRoleEntity>(e => e.Id == clubRoleId), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutClubRoleAsync(_clubId, clubRoleId, clubRole, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PatchClubRoleAsync

        [TestMethod]
        public async Task PatchClubRoleAsync_SuccessScenario_ShouldReturnClubRoleDto()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            ClubRole clubRole = _dtoBuilder.WithId(clubRoleId).WithClubId(_clubId).Build();
            string updatedName = "Updated Club Role Name";
            ActiveStatus updatedStatus = ActiveStatus.Inactive;
            List<PropertyUpdate> propertyUpdates =
            [
                new() { Property = nameof(ClubRole.Name), Value = updatedName },
                new() { Property = nameof(ClubRole.ActiveStatus), Value = updatedStatus },
            ];
            ClubRoleEntity updatedEntity = clubRole.ToClubRoleEntity();
            updatedEntity.Name = updatedName;
            updatedEntity.ActiveStatus = updatedStatus;

            _clubRoleServiceMock
                .Setup(s => s.PatchAsync(_clubId, clubRoleId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(updatedEntity))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubRoleAsync(_clubId, clubRoleId, propertyUpdates, _cancellationToken);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubRoleResponse = result.Value;
            Assert.IsNotNull(clubRoleResponse);
            Assert.AreEqual(clubRoleId, clubRoleResponse.Id);
            Assert.AreEqual(updatedName, clubRoleResponse.Name);
            Assert.AreEqual(updatedStatus, clubRoleResponse.ActiveStatus);
            Assert.IsInstanceOfType<ClubRole>(clubRoleResponse);
        }

        [TestMethod]
        public async Task PatchClubRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _clubRoleServiceMock
                .Setup(s => s.PatchAsync(_clubId, clubRoleId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubRoleAsync(_clubId, clubRoleId, [], _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PatchClubRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();

            _clubRoleServiceMock
                .Setup(s => s.PatchAsync(_clubId, clubRoleId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubRoleAsync(_clubId, clubRoleId, [], _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region DeleteClubRoleAsync

        [TestMethod]
        public async Task DeleteClubRoleAsync_SuccessScenario_ShouldReturnNoContent()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();

            _clubRoleServiceMock
                .Setup(s => s.DeleteAsync(_clubId, clubRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(_entityBuilder.WithId(clubRoleId).WithClubId(_clubId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubRoleAsync(_clubId, clubRoleId, _cancellationToken);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<NoContentResult>(result);
            Assert.AreEqual(204, ((NoContentResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubRoleAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _clubRoleServiceMock
                .Setup(s => s.DeleteAsync(_clubId, clubRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubRoleAsync(_clubId, clubRoleId, _cancellationToken);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubRoleAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubRoleId = Guid.NewGuid();

            _clubRoleServiceMock
                .Setup(s => s.DeleteAsync(_clubId, clubRoleId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubRoleEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubRoleAsync(_clubId, clubRoleId, _cancellationToken);

            // Assert
            _clubRoleServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<ObjectResult>(result);
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        #endregion
    }
}
