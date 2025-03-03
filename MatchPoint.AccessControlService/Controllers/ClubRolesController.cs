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
    public class ClubRolesController(IClubRoleService _clubRoleService, ILogger<ClubRolesController> _logger)
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
            var result = await _clubRoleService.GetAllWithSpecificationAsync(
                clubId,
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

        // GET: api/v1/clubs/[guid]/roles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("ClubRoles.Read")]
        [HttpGet("{id:guid}", Name = nameof(GetClubRoleAsync))]
        public async Task<ActionResult<ClubRole>> GetClubRoleAsync(Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received GET request to retrieve club role with ID: {Id}", id);
            var result = await _clubRoleService.GetByIdAsync(clubId, id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to retrieve club role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully retrieved club role with ID: {Id}", id);

            return result.Data.ToClubRoleDto();
        }

        // POST: api/v1/clubs/[guid]/roles
        [MapToApiVersion(1)]
        [RequiredScope("ClubRoles.Write")]
        [HttpPost]
        public async Task<ActionResult<Club>> PostClubRoleAsync(Guid clubId, ClubRole clubRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received POST request to CREATE club role for club '{clubId}' with name: {clubRoleName}",
                clubId, clubRole.Name);

            var result = await _clubRoleService.CreateAsync(clubId, clubRole.ToClubRoleEntity(), cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to create club role with name: {clubRoleName}. Error: {Error}",
                    clubRole.Name, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully created club role with ID: {Id}, name: {clubRoleName}",
                result.Data.Id, result.Data.Name);

            var apiVersion = HttpContext.GetRequestedApiVersion()?.ToString();
            return CreatedAtRoute(
                nameof(GetClubRoleAsync),
                new { version = apiVersion, clubId, id = result.Data.Id.ToString() },
                result.Data.ToClubRoleDto());
        }

        // PUT: api/v1/clubs/[guid]/roles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("ClubRoles.Write")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ClubRole>> PutClubRoleAsync(
            Guid clubId, Guid id, ClubRole clubRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received PUT request to UPDATE club role with ID: {Id} for club '{clubId}'", 
                id, clubId);
            if (id != clubRole.Id)
            {
                string errorMsg = $"Ids '{id}' and '{clubRole.Id}' do not match.";
                _logger.LogWarning("Failed to update club role with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _clubRoleService.UpdateAsync(clubId, clubRole.ToClubRoleEntity(), cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update club role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully updated club role with ID: {Id} for club '{clubId}'", 
                id, clubId);

            return result.Data.ToClubRoleDto();
        }

        // PATCH: api/v1/clubs/[guid]/roles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("ClubRoles.Write")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ClubRole>> PatchClubRoleAsync(
            Guid clubId, Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received PATCH request to UPDATE {count} properties for club role with ID: {Id}",
                propertyUpdates.Count(), id);
            if (id == default)
            {
                string errorMsg = $"Id '{id}' is not valid.";
                _logger.LogWarning("Failed to update club role with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _clubRoleService.PatchAsync(clubId, id, propertyUpdates, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update club role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully updated club role with ID: {Id}", id);

            return result.Data.ToClubRoleDto();
        }

        // DELETE: api/v1/clubs/[guid]/roles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("ClubRoles.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClubRoleAsync(Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received DELETE request to delete club role with ID: {Id}", id);
            var result = await _clubRoleService.DeleteAsync(clubId, id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to delete club role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully deleted club role with ID: {Id}", id);
            return NoContent();
        }
    }
}
