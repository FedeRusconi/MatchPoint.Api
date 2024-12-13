using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MatchPoint.Api.Tests.Shared.Common.Helpers
{
    public class ActionResultHelpers
    {
        /// <summary>
        /// Extract the numerical status code from an <see cref="ActionResult"/>
        /// </summary>
        public static int? ExtractStatusCode<T>(ActionResult<T> actionResult)
        {
            IConvertToActionResult convertToActionResult = actionResult;
            var actionResultWithStatusCode = convertToActionResult.Convert() as IStatusCodeActionResult;
            return actionResultWithStatusCode?.StatusCode;
        }
    }
}
