using MatchPoint.ClubService.Authorization.Requirements;
using MatchPoint.ClubService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using System.Reflection.Metadata;

namespace MatchPoint.ClubService.Authorization.Handlers
{
    public class ClubAuthorizationHandler : AuthorizationHandler<ClubAdminRequirement, ClubEntity>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ClubAdminRequirement requirement,
            ClubEntity resource)
        {
            var currentUserId = context.User.GetObjectId();
            //if (resource.Staff.Any())
            //{
            //    context.Succeed(requirement);
            //}

            return Task.CompletedTask;
        }
    }
}
