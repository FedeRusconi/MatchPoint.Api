using Asp.Versioning;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace MatchPoint.AccessControlService.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/clubs/{clubId:guid}/roles")]
    [ApiController]
    [Authorize]
    public class ClubRolesController(IClubRoleService _customRoleService, ILogger<ClubRolesController> _logger)
        : ControllerBase
    {
        // GET: api/v1/clubs/[guid]/roles
        [MapToApiVersion(1)]
        [RequiredScope("ClubRoles.Read")]
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ClubRole>>> GetClubRolesAsync(
            Guid clubId,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = Constants.MaxPageSizeAllowed,
            [FromQuery] Dictionary<string, string>? filters = null,
            [FromQuery] Dictionary<string, SortDirection>? orderBy = null)
        {
            _logger.LogInformation(
                "Received GET request to retrieve club roles for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}",
                page, pageSize, filters?.Count, orderBy?.First().Key);
            var result = await _customRoleService.GetAllWithSpecificationAsync(
                //clubId,
                pageNumber: page,
                pageSize: pageSize,
                filters: filters,
                orderBy: orderBy,
                cancellationToken: cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to retrieve club roles for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}. " +
                    "Error: {Error}",
                    page, pageSize, filters?.Count, orderBy?.First().Key, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully found {totalCount} club roles with filters: {countFilters}, orderBy: {orderBy}. " +
                "Returning page {page} with up to {pageSize} club roles",
                result.Data.TotalCount, filters?.Count, orderBy?.First().Key, page, pageSize);

            return new PagedResponse<ClubRole>()
            {
                CurrentPage = result.Data.CurrentPage,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount,
                Data = result.Data.Data.ToClubRoleDtoEnumerable()
            };
        }
    }
}
