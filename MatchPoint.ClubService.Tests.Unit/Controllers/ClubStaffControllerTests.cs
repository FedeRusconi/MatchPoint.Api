using Asp.Versioning;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using MatchPoint.ClubService.Controllers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Mappers;
using MatchPoint.ClubService.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MatchPoint.ClubService.Tests.Unit.Controllers
{
    [TestClass]
    public class ClubStaffControllerTests
    {
        private Mock<IClubStaffService> _clubStaffServiceMock = default!;
        private Mock<ILogger<ClubStaffController>> _loggerMock = default!;
        private ClubStaffController _controller = default!;
        private ClubStaffEntityBuilder _entityBuilder = default!;
        private ClubStaffBuilder _dtoBuilder = default!;
        private readonly Guid _clubId = Guid.NewGuid();

        [TestInitialize]
        public void TestInitialize()
        {
            _clubStaffServiceMock = new();
            _loggerMock = new();
            _controller = new ClubStaffController(_clubStaffServiceMock.Object, _loggerMock.Object)
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
                .Returns(new ApiVersion(ClubServiceEndpoints.CurrentVersion));
            _controller.ControllerContext.HttpContext.Features.Set(apiVersionFeature.Object);

            _entityBuilder = new ClubStaffEntityBuilder();
            _dtoBuilder = new ClubStaffBuilder();
        }

        #region GetClubStaffAsync

        [TestMethod]
        public async Task GetClubStaffAsync_SuccessScenario_ShouldReturnConvertedPagedResponse()
        {
            // Arrange
            PagedResponse<ClubStaffEntity> serviceResponse = new()
            {
                CurrentPage = 1,
                PageSize = 10,
                TotalCount = 10,
                Data = [_entityBuilder.Build()]
            };
            _clubStaffServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(_clubId, 1, Constants.MaxPageSizeAllowed, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubStaffEntity>>.Success(serviceResponse))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubStaffAsync(_clubId);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var pagedResponse = result.Value;
            Assert.IsNotNull(pagedResponse);
            Assert.AreEqual(serviceResponse.CurrentPage, pagedResponse.CurrentPage);
            Assert.AreEqual(serviceResponse.PageSize, pagedResponse.PageSize);
            Assert.AreEqual(serviceResponse.TotalCount, pagedResponse.TotalCount);
            Assert.IsInstanceOfType<IEnumerable<ClubStaff>>(pagedResponse.Data);
        }

        [TestMethod]
        public async Task GetClubStaffAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubStaffServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(_clubId, 1, Constants.MaxPageSizeAllowed, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubStaffEntity>>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubStaffAsync(_clubId);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetClubStaffAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            _clubStaffServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(_clubId, 1, Constants.MaxPageSizeAllowed, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubStaffEntity>>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubStaffAsync(_clubId);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region GetSingleClubStaffAsync

        [TestMethod]
        public async Task GetSingleClubStaffAsync_SuccessScenario_ShouldReturnClubDto()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            _clubStaffServiceMock
                .Setup(s => s.GetByIdAsync(_clubId, clubStaffId))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(_entityBuilder.WithId(clubStaffId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetSingleClubStaffAsync(_clubId, clubStaffId);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubStaff = result.Value;
            Assert.IsNotNull(clubStaff);
            Assert.AreEqual(clubStaffId, clubStaff.Id);
            Assert.IsInstanceOfType<ClubStaff>(clubStaff);
        }

        [TestMethod]
        public async Task GetSingleClubStaffAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubStaffServiceMock
                .Setup(s => s.GetByIdAsync(_clubId, clubStaffId))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetSingleClubStaffAsync(_clubId, clubStaffId);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetSingleClubStaffAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            _clubStaffServiceMock
                .Setup(s => s.GetByIdAsync(_clubId, clubStaffId))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetSingleClubStaffAsync(_clubId, clubStaffId);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PostClubStaffAsync

        [TestMethod]
        public async Task PostClubStaffAsync_SuccessScenario_ShouldReturnClubStaffDto()
        {
            // Arrange
            ClubStaff clubStaff = _dtoBuilder.Build();
            _clubStaffServiceMock
                .Setup(s => s.CreateAsync(It.Is<ClubStaffEntity>(e => e.Id == clubStaff.Id)))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(clubStaff.ToClubStaffEntity()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubStaffAsync(_clubId, clubStaff);
            var x = result.GetType();

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubStaffResponse = ((ObjectResult)result.Result!).Value as ClubStaff;
            Assert.IsNotNull(clubStaffResponse);
            Assert.AreEqual(clubStaff.Id, clubStaffResponse.Id);
            Assert.IsInstanceOfType<ClubStaff>(clubStaffResponse);
        }

        [TestMethod]
        public async Task PostClubStaffAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            ClubStaff clubStaff = _dtoBuilder.Build();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubStaffServiceMock
                .Setup(s => s.CreateAsync(It.Is<ClubStaffEntity>(e => e.Id == clubStaff.Id)))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubStaffAsync(_clubId, clubStaff);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PostClubStaffAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            ClubStaff clubStaff = _dtoBuilder.Build();
            _clubStaffServiceMock
                .Setup(s => s.CreateAsync(It.Is<ClubStaffEntity>(e => e.Id == clubStaff.Id)))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubStaffAsync(_clubId, clubStaff);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PatchClubStaffAsync

        [TestMethod]
        public async Task PatchClubStaffAsync_SuccessScenario_ShouldReturnClubStaffDto()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            ClubStaff clubStaff = _dtoBuilder.WithId(clubStaffId).Build();
            string updatedFirstName = "Updated First Name";
            string updatedPhone = "Updated Phone Number";
            List<PropertyUpdate> propertyUpdates =
            [
                new() { Property = nameof(ClubStaff.FirstName), Value = updatedFirstName },
                new() { Property = nameof(ClubStaff.PhoneNumber), Value = updatedPhone },
            ];
            ClubStaffEntity updatedEntity = clubStaff.ToClubStaffEntity();
            updatedEntity.FirstName = updatedFirstName;
            updatedEntity.PhoneNumber = updatedPhone;

            _clubStaffServiceMock
                .Setup(s => s.PatchAsync(clubStaffId, It.IsAny<IEnumerable<PropertyUpdate>>()))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(updatedEntity))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubStaffAsync(clubStaffId, propertyUpdates);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubStaffResponse = result.Value;
            Assert.IsNotNull(clubStaffResponse);
            Assert.AreEqual(clubStaffId, clubStaffResponse.Id);
            Assert.AreEqual(updatedFirstName, clubStaffResponse.FirstName);
            Assert.AreEqual(updatedPhone, clubStaffResponse.PhoneNumber);
            Assert.IsInstanceOfType<ClubStaff>(clubStaffResponse);
        }

        [TestMethod]
        public async Task PatchClubStaffAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _clubStaffServiceMock
                .Setup(s => s.PatchAsync(clubStaffId, It.IsAny<IEnumerable<PropertyUpdate>>()))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubStaffAsync(clubStaffId, []);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PatchClubStaffAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();

            _clubStaffServiceMock
                .Setup(s => s.PatchAsync(clubStaffId, It.IsAny<IEnumerable<PropertyUpdate>>()))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubStaffAsync(clubStaffId, []);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region DeleteClubStaffAsync

        [TestMethod]
        public async Task DeleteClubStaffAsync_SuccessScenario_ShouldReturnNoContent()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();

            _clubStaffServiceMock
                .Setup(s => s.DeleteAsync(clubStaffId))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(_entityBuilder.WithId(clubStaffId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubStaffAsync(clubStaffId);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<NoContentResult>(result);
            Assert.AreEqual(204, ((NoContentResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubStaffAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _clubStaffServiceMock
                .Setup(s => s.DeleteAsync(clubStaffId))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubStaffAsync(clubStaffId);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubStaffAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubStaffId = Guid.NewGuid();

            _clubStaffServiceMock
                .Setup(s => s.DeleteAsync(clubStaffId))
                .ReturnsAsync(ServiceResult<ClubStaffEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubStaffAsync(clubStaffId);

            // Assert
            _clubStaffServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<ObjectResult>(result);
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        #endregion
    }
}
