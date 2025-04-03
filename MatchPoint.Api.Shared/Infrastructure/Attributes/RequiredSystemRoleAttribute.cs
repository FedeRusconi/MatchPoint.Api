using MatchPoint.Api.Shared.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchPoint.Api.Shared.Infrastructure.Attributes
{
    /// <summary>
    /// This attribute is used to implement to allowing only users who have a SystemRole to endpoints.
    /// </summary>
    /// <param name="_systemRole"> The <see cref="SystemRole"/> to check for. </param>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiredSystemRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly SystemRole? _systemRole;

        public RequiredSystemRoleAttribute()
        {
        }

        public RequiredSystemRoleAttribute(SystemRole systemRole)
        {
            _systemRole = systemRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var systemRoleString = context.HttpContext.User.FindFirst("extension_SystemRole")?.Value;
            if (string.IsNullOrEmpty(systemRoleString) || !Enum.TryParse<SystemRole>(systemRoleString, out var systemRole))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                return;
            }

            // If SystemRole was provided, ensure user has that exact role
            if (_systemRole.HasValue && systemRole != _systemRole)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
            // If SystemRole was not provided, restrict only SytemRole.None
            if (!_systemRole.HasValue && systemRole == SystemRole.None)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
        }
    }
}
