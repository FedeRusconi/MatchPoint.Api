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
    public class ClubsControllerTests
    {
        private Mock<IClubManagementService> _clubServiceMock = default!;
        private Mock<ILogger<ClubsController>> _loggerMock = default!;
        private ClubsController _controller = default!;
        private ClubEntityBuilder _entityBuilder = default!;
        private ClubBuilder _dtoBuilder = default!;
        private CancellationToken _cancellationToken = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _clubServiceMock = new();
            _loggerMock = new();
            _controller = new ClubsController(_clubServiceMock.Object, _loggerMock.Object)
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

            _entityBuilder = new ClubEntityBuilder();
            _dtoBuilder = new ClubBuilder();
            _cancellationToken = new CancellationToken();
        }

        #region GetClubsAsync

        [TestMethod]
        public async Task GetClubsAsync_SuccessScenario_ShouldReturnConvertedPagedResponse()
        {
            // Arrange
            PagedResponse<ClubEntity> serviceResponse = new()
            {
                CurrentPage = 1,
                PageSize = 10,
                TotalCount = 10,
                Data = [_entityBuilder.Build()]
            };
            _clubServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(
                    1, Constants.MaxPageSizeAllowed, _cancellationToken, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubEntity>>.Success(serviceResponse))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubsAsync(_cancellationToken);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var pagedResponse = result.Value;
            Assert.IsNotNull(pagedResponse);
            Assert.AreEqual(serviceResponse.CurrentPage, pagedResponse.CurrentPage);
            Assert.AreEqual(serviceResponse.PageSize, pagedResponse.PageSize);
            Assert.AreEqual(serviceResponse.TotalCount, pagedResponse.TotalCount);
            Assert.IsInstanceOfType<IEnumerable<Club>>(pagedResponse.Data);
        }

        [TestMethod]
        public async Task GetClubsAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(1, Constants.MaxPageSizeAllowed, _cancellationToken, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubEntity>>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubsAsync(_cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetClubsAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            _clubServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(
                    1, Constants.MaxPageSizeAllowed, _cancellationToken, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubEntity>>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubsAsync(_cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region GetClubAsync

        [TestMethod]
        public async Task GetClubAsync_SuccessScenario_ShouldReturnClubDto()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            _clubServiceMock
                .Setup(s => s.GetByIdAsync(clubId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(_entityBuilder.WithId(clubId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubAsync(clubId, _cancellationToken);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var club = result.Value;
            Assert.IsNotNull(club);
            Assert.AreEqual(clubId, club.Id);
            Assert.IsInstanceOfType<Club>(club);
        }

        [TestMethod]
        public async Task GetClubAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubServiceMock
                .Setup(s => s.GetByIdAsync(clubId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubAsync(clubId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task GetClubAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            _clubServiceMock
                .Setup(s => s.GetByIdAsync(clubId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.GetClubAsync(clubId, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PostClubAsync

        [TestMethod]
        public async Task PostClubAsync_SuccessScenario_ShouldReturnClubDto()
        {
            // Arrange
            Club club = _dtoBuilder.Build();
            _clubServiceMock
                .Setup(s => s.CreateAsync(It.Is<ClubEntity>(e => e.Id == club.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(club.ToClubEntity()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubAsync(club, _cancellationToken);
            var x = result.GetType();

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubResponse = ((ObjectResult)result.Result!).Value as Club;
            Assert.IsNotNull(clubResponse);
            Assert.AreEqual(club.Id, clubResponse.Id);
            Assert.IsInstanceOfType<Club>(clubResponse);
        }

        [TestMethod]
        public async Task PostClubAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Club club = _dtoBuilder.Build();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubServiceMock
                .Setup(s => s.CreateAsync(It.Is<ClubEntity>(e => e.Id == club.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubAsync(club, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PostClubAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Club club = _dtoBuilder.Build();
            _clubServiceMock
                .Setup(s => s.CreateAsync(It.Is<ClubEntity>(e => e.Id == club.Id), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PostClubAsync(club, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PutClubAsync

        [TestMethod]
        public async Task PutClubAsync_SuccessScenario_ShouldReturnClubDto()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            Club club = _dtoBuilder.WithId(clubId).Build();
            _clubServiceMock
                .Setup(s => s.UpdateAsync(It.Is<ClubEntity>(e => e.Id == clubId), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(club.ToClubEntity()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutClubAsync(clubId, club, _cancellationToken);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubResponse = result.Value;
            Assert.IsNotNull(clubResponse);
            Assert.AreEqual(clubId, clubResponse.Id);
            Assert.IsInstanceOfType<Club>(clubResponse);
        }

        [TestMethod]
        public async Task PutClubAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            Club club = _dtoBuilder.WithId(clubId).Build();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubServiceMock
                .Setup(s => s.UpdateAsync(It.Is<ClubEntity>(e => e.Id == clubId), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutClubAsync(clubId, club, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PutClubAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            Club club = _dtoBuilder.WithId(clubId).Build();
            _clubServiceMock
                .Setup(s => s.UpdateAsync(It.Is<ClubEntity>(e => e.Id == clubId), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PutClubAsync(clubId, club, _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region PatchClubAsync

        [TestMethod]
        public async Task PatchClubAsync_SuccessScenario_ShouldReturnClubDto()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            Club club = _dtoBuilder.WithId(clubId).Build();
            string updatedName = "Updated Club Name";
            string updatedEmail = "updated@email.com";
            List<PropertyUpdate> propertyUpdates =
            [
                new() { Property = nameof(Club.Name), Value = updatedName },
                new() { Property = nameof(Club.Email), Value = updatedEmail },
            ];
            ClubEntity updatedEntity = club.ToClubEntity();
            updatedEntity.Name = updatedName;
            updatedEntity.Email = updatedEmail;

            _clubServiceMock
                .Setup(s => s.PatchAsync(clubId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(updatedEntity))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubAsync(clubId, propertyUpdates, _cancellationToken);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var clubResponse = result.Value;
            Assert.IsNotNull(clubResponse);
            Assert.AreEqual(clubId, clubResponse.Id);
            Assert.AreEqual(updatedName, clubResponse.Name);
            Assert.AreEqual(updatedEmail, clubResponse.Email);
            Assert.IsInstanceOfType<Club>(clubResponse);
        }

        [TestMethod]
        public async Task PatchClubAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _clubServiceMock
                .Setup(s => s.PatchAsync(clubId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubAsync(clubId, [], _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public async Task PatchClubAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();

            _clubServiceMock
                .Setup(s => s.PatchAsync(clubId, It.IsAny<IEnumerable<PropertyUpdate>>(), _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.PatchClubAsync(clubId, [], _cancellationToken);
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(500, statusCode);
        }

        #endregion

        #region DeleteClubAsync

        [TestMethod]
        public async Task DeleteClubAsync_SuccessScenario_ShouldReturnNoContent()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();

            _clubServiceMock
                .Setup(s => s.DeleteAsync(clubId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(_entityBuilder.WithId(clubId).Build()))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubAsync(clubId, _cancellationToken);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<NoContentResult>(result);
            Assert.AreEqual(204, ((NoContentResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;

            _clubServiceMock
                .Setup(s => s.DeleteAsync(clubId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubAsync(clubId, _cancellationToken);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task DeleteClubAsync_NullResultData_ShouldReturnAppropriateResponseCode()
        {
            // Arrange
            Guid clubId = Guid.NewGuid();

            _clubServiceMock
                .Setup(s => s.DeleteAsync(clubId, _cancellationToken))
                .ReturnsAsync(ServiceResult<ClubEntity>.Success(null!))
                .Verifiable(Times.Once);

            // Act
            var result = await _controller.DeleteClubAsync(clubId, _cancellationToken);

            // Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<ObjectResult>(result);
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        #endregion

    }
}
