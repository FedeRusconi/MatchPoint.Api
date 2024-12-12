using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.Api.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MatchPoint.Api.Tests.Unit.Infrastructure.Extensions
{
    [TestClass]
    public class IServiceResultExtensionsTests
    {
        private GenericControllerTest _controller = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _controller = new GenericControllerTest();
        }

        [TestMethod]
        public void ToFailureActionResult_WhenResultIsBadRequest_ShouldReturnBadRequestObjectResult()
        {
            #region Arrange
            string errorMsg = "Some error here";
            int expectedCode = 400;
            var result = ServiceResult<GenericEntityTest>.Failure(errorMsg, ServiceResultType.BadRequest);
            #endregion

            #region Act
            var actionResult = result.ToFailureActionResult(_controller);
            #endregion

            #region Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
            var objectResult = (BadRequestObjectResult)actionResult;
            Assert.AreEqual(expectedCode, objectResult.StatusCode);
            Assert.AreEqual(errorMsg, objectResult.Value);
            #endregion
        }

        [TestMethod]
        public void ToFailureActionResult_WhenResultIsUnauthorized_ShouldReturnUnauthorizedObjectResult()
        {
            #region Arrange
            string errorMsg = "Some error here";
            int expectedCode = 401;
            var result = ServiceResult<GenericEntityTest>.Failure(errorMsg, ServiceResultType.Unauthorized);
            #endregion

            #region Act
            var actionResult = result.ToFailureActionResult(_controller);
            #endregion

            #region Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType<UnauthorizedObjectResult>(actionResult);
            var objectResult = (UnauthorizedObjectResult)actionResult;
            Assert.AreEqual(expectedCode, objectResult.StatusCode);
            Assert.AreEqual(errorMsg, objectResult.Value);
            #endregion
        }

        [TestMethod]
        public void ToFailureActionResult_WhenResultIsForbidden_ShouldReturnForbidResult()
        {
            #region Arrange
            string errorMsg = "Some error here";
            var result = ServiceResult<GenericEntityTest>.Failure(errorMsg, ServiceResultType.Forbidden);
            #endregion

            #region Act
            var actionResult = result.ToFailureActionResult(_controller);
            #endregion

            #region Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType<ForbidResult>(actionResult);
            var objectResult = (ForbidResult)actionResult;
            Assert.AreEqual(errorMsg, objectResult.AuthenticationSchemes.First());
            #endregion
        }

        [TestMethod]
        public void ToFailureActionResult_WhenResultIsNotFound_ShouldReturnNotFoundObjectResult()
        {
            #region Arrange
            string errorMsg = "Some error here";
            int expectedCode = 404;
            var result = ServiceResult<GenericEntityTest>.Failure(errorMsg, ServiceResultType.NotFound);
            #endregion

            #region Act
            var actionResult = result.ToFailureActionResult(_controller);
            #endregion

            #region Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult);
            var objectResult = (NotFoundObjectResult)actionResult;
            Assert.AreEqual(expectedCode, objectResult.StatusCode);
            Assert.AreEqual(errorMsg, objectResult.Value);
            #endregion
        }

        [TestMethod]
        public void ToFailureActionResult_WhenResultIsConflict_ShouldReturnConflictObjectResult()
        {
            #region Arrange
            string errorMsg = "Some error here";
            int expectedCode = 409;
            var result = ServiceResult<GenericEntityTest>.Failure(errorMsg, ServiceResultType.Conflict);
            #endregion

            #region Act
            var actionResult = result.ToFailureActionResult(_controller);
            #endregion

            #region Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType<ConflictObjectResult>(actionResult);
            var objectResult = (ConflictObjectResult)actionResult;
            Assert.AreEqual(expectedCode, objectResult.StatusCode);
            Assert.AreEqual(errorMsg, objectResult.Value);
            #endregion
        }

        [TestMethod]
        public void ToFailureActionResult_WhenResultIsInternalError_ShouldReturnObjectResult()
        {
            #region Arrange
            string errorMsg = "Some error here";
            int expectedCode = 500;
            var result = ServiceResult<GenericEntityTest>.Failure(errorMsg, ServiceResultType.InternalError);
            #endregion

            #region Act
            var actionResult = result.ToFailureActionResult(_controller);
            #endregion

            #region Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType<ObjectResult>(actionResult);
            var objectResult = (ObjectResult)actionResult;
            Assert.AreEqual(expectedCode, objectResult.StatusCode);
            Assert.AreEqual(errorMsg, objectResult.Value);
            #endregion
        }

        [TestMethod]
        public void ToFailureActionResult_WhenResultTypeIsUnrecognized_ShouldReturnObjectResult()
        {
            #region Arrange
            string errorMsg = "Some error here";
            int expectedCode = 500;
            var result = ServiceResult<GenericEntityTest>.Failure(errorMsg, (ServiceResultType)1000);
            #endregion

            #region Act
            var actionResult = result.ToFailureActionResult(_controller);
            #endregion

            #region Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType<ObjectResult>(actionResult);
            var objectResult = (ObjectResult)actionResult;
            Assert.AreEqual(expectedCode, objectResult.StatusCode);
            Assert.AreEqual(errorMsg, objectResult.Value);
            #endregion
        }
    }
}
