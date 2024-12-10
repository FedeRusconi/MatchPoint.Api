using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MatchPoint.Api.Shared.Infrastructure.Extensions
{
    public static class IServiceResultExtensions
    {
        public static ActionResult ToFailureActionResult<T>(
            this IServiceResult<T> serviceResult, ControllerBase controller)
        {
            return serviceResult.ResultType switch
            {
                ServiceResultType.NotFound => controller.NotFound(serviceResult.Error),
                ServiceResultType.Conflict => controller.Conflict(serviceResult.Error),
                ServiceResultType.BadRequest => controller.BadRequest(serviceResult.Error),
                ServiceResultType.Unauthorized => controller.Unauthorized(serviceResult.Error),
                ServiceResultType.Forbidden => controller.Forbid(serviceResult.Error),
                _ => controller.StatusCode(500, serviceResult.Error)
            };
        }
    }
}
