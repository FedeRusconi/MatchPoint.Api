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

        [TestInitialize]
        public void TestInitialize()
        {
            _clubServiceMock = new();
            _loggerMock = new();
            _controller = new ClubsController(_clubServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new()
            };
            _entityBuilder = new ClubEntityBuilder();
        }

        #region GetClubsAsync

        [TestMethod]
        public async Task GetClubsAsync_SuccessScenario_ShouldReturnConvertedPagedResponse()
        {
            #region Arrange
            PagedResponse<ClubEntity> serviceResponse = new()
            {
                CurrentPage = 1,
                PageSize = 10,
                TotalCount = 10,
                Data = [_entityBuilder.Build()]
            };
            _clubServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(1, Constants.MaxPageSizeAllowed, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubEntity>>.Success(serviceResponse))
                .Verifiable(Times.Once);
            #endregion

            #region Act
            var result = await _controller.GetClubsAsync();
            #endregion

            #region Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            var pagedResponse = result.Value;
            Assert.IsNotNull(pagedResponse);
            Assert.AreEqual(serviceResponse.CurrentPage, pagedResponse.CurrentPage);
            Assert.AreEqual(serviceResponse.PageSize, pagedResponse.PageSize);
            Assert.AreEqual(serviceResponse.TotalCount, pagedResponse.TotalCount);
            Assert.IsInstanceOfType<IEnumerable<Club>>(pagedResponse.Data);
            #endregion
        }

        [TestMethod]
        public async Task GetClubsAsync_FailScenario_ShouldReturnAppropriateResponseCode()
        {
            #region Arrange
            string errorMsg = "This is a test error";
            ServiceResultType resultType = ServiceResultType.BadRequest;
            _clubServiceMock
                .Setup(s => s.GetAllWithSpecificationAsync(1, Constants.MaxPageSizeAllowed, null, null))
                .ReturnsAsync(ServiceResult<PagedResponse<ClubEntity>>.Failure(errorMsg, resultType))
                .Verifiable(Times.Once);
            #endregion

            #region Act
            var result = await _controller.GetClubsAsync();
            var statusCode = ActionResultHelpers.ExtractStatusCode(result);
            #endregion

            #region Assert
            _clubServiceMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
            Assert.AreEqual(400, statusCode);
            #endregion
        }

        #endregion

    }
}
