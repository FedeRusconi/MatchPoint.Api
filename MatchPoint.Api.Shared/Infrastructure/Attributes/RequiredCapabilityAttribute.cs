﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.ClubService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace MatchPoint.Api.Shared.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiredCapabilityAttribute(RoleCapabilityFeature _feature, RoleCapabilityAction _action)
        : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var service = new RequiredCapabilityService(context);

            // Bypass checks if user is system admin
            if (service.IsSystemAdmin()) return;

            // Extract Ids
            if (!service.TryGetClubId(out Guid clubId) || !service.TryGetRoleId(out Guid roleId))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            // Find ClubRole
            var clubRole = await service.GetClubRoleAsync(clubId, roleId);
            if (clubRole == null)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            // Find required capability and ensure user has enough permissions
            var capability = clubRole.Capabilities.FirstOrDefault(c => c.Feature == _feature);
            if (capability == null || capability.Action < _action)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }

    public class RequiredCapabilityService(AuthorizationFilterContext _context)
    {
        private readonly IHttpClientFactory _httpClientFactory = _context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
        private readonly HybridCache _hybridCache = _context.HttpContext.RequestServices.GetRequiredService<HybridCache>();

        /// <summary>
        /// NOTE: This method temporarily returns always False, to allow the other checks to happen.
        /// This will be implemented when the admin app is built. 
        /// Easiest option seem to be adding new custom attributes to Aure Ad b2c users for IsAdmin and IsSuperAdmin
        /// </summary>
        /// <returns>
        /// <c>true</c> if current user is System Admin or SuperAdmin.
        /// </returns>
        public bool IsSystemAdmin()
        {
            return false;
        }

        /// <summary>
        /// Try to extract club id from incoming request and output reesult.
        /// </summary>
        /// <param name="clubId">
        /// The <see cref="Guid"/> extracted belonging to the <see cref="Club"/>.
        /// </param>
        /// <returns> <c>True</c> if club id was extracted successfully. <c>False</c> otherwise. </returns>
        public bool TryGetClubId(out Guid clubId)
        {
            var clubIdString = _context.HttpContext.Request.Path.Value?.Split('/')[4];
            if (string.IsNullOrEmpty(clubIdString) || !Guid.TryParse(clubIdString, out clubId))
            {
                clubId = Guid.Empty;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Try to extract role id from incoming request and output reesult.
        /// </summary>
        /// <param name="roleId">
        /// The <see cref="Guid"/> extracted belonging to the <see cref="ClubRole"/>.
        /// </param>
        /// <returns> <c>True</c> if role id was extracted successfully. <c>False</c> otherwise. </returns>
        public bool TryGetRoleId(out Guid roleId)
        {
            var roleIdString = _context.HttpContext.User.FindFirst("extension_RoleId")?.Value;
            if (string.IsNullOrEmpty(roleIdString) || !Guid.TryParse(roleIdString, out roleId))
            {
                roleId = Guid.Empty;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Find the <see cref="ClubRole"/> from provided clubId and with provided roleId.
        /// </summary>
        /// <param name="clubId"> The <see cref="Guid"/> of the selected the <see cref="Club"/>. </param>
        /// <param name="roleId"> The <see cref="Guid"/> of the selecte <see cref="ClubRole"/>. </param>
        /// <returns> The given <see cref="ClubRole"/> or <c>null</c> if not found. </returns>
        public async Task<ClubRole?> GetClubRoleAsync(Guid clubId, Guid roleId)
        {
            // Set http client
            var client = _httpClientFactory.CreateClient("AccessControlService");
            var userToken = _context.HttpContext.Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", userToken.ToString().Replace("Bearer ", ""));
            // Set cache
            string cacheKey = $"clubRole_{clubId}_{roleId}";
            string[] cacheTags = [clubId.ToString(), roleId.ToString()];

            // Return ClubRole found or null
            try
            {
                return await _hybridCache.GetOrCreateAsync(
                    cacheKey,
                    async cancel => await client.GetFromJsonAsync<ClubRole>($"api/v1/clubs/{clubId}/roles/{roleId}", cancel),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7) }, // Simplified to just Expiration
                    tags: cacheTags,
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
