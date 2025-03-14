using System.Net.Http.Headers;
using System.Net.Http.Json;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.AccessControlService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace MatchPoint.Api.Shared.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireCapabilityAttribute(RoleCapabilityFeature _feature, RoleCapabilityAction _action)
        : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Get required services
            var httpClientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
            var hybridCache = context.HttpContext.RequestServices.GetRequiredService<HybridCache>();
            
            // Extract club id from URL and role id from token claims
            var clubIdString = context.HttpContext.Request.Path.Value?.Split('/')[4];
            var roleIdString = context.HttpContext.User.FindFirst("extension_RoleId")?.Value;
            if (string.IsNullOrEmpty(clubIdString) || !Guid.TryParse(clubIdString, out Guid clubId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (string.IsNullOrEmpty(roleIdString) || !Guid.TryParse(roleIdString, out Guid roleId))
            {
                context.Result = new ForbidResult("Invalid role id");
                return;
            }

            // Prepare http client for accesscontrolservice and re-use incoming token
            var client = httpClientFactory.CreateClient("AccessControlService");
            var userToken = context.HttpContext.Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", userToken.ToString().Replace("Bearer ", ""));

            // Set cache
            string cacheKey = $"clubRole_{clubId}_{roleId}";
            string[] cacheTags = [clubIdString, roleIdString];

            // Find club role
            var clubRole = await hybridCache.GetOrCreateAsync(
                cacheKey,
                async cancel => await client.GetFromJsonAsync<ClubRole>($"api/v1/clubs/{clubId}/roles/{roleId}", cancellationToken: cancel),
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromDays(7),
                    LocalCacheExpiration = TimeSpan.FromDays(7)
                },
                tags: cacheTags,
                cancellationToken: CancellationToken.None);
            if (clubRole == null)
            {
                context.Result = new ForbidResult("Invalid club role");
                return;
            }

            // Find required capability and ensure user has enough permissions
            var capability = clubRole.Capabilities.FirstOrDefault(c => c.Feature == _feature);
            if (capability == null || capability.Action < _action)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
