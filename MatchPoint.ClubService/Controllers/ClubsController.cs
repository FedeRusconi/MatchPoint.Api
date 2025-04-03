using Asp.Versioning;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Attributes;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace MatchPoint.ClubService.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ClubsController(IClubManagementService _clubService, ILogger<ClubsController> _logger) : ControllerBase
    {
        // GET: api/v1/clubs
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.Clubs}.{Scopes.Read}")]
        [HttpGet]
        public async Task<ActionResult<PagedResponse<Club>>> GetClubsAsync(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = Constants.MaxPageSizeAllowed,
            [FromQuery] Dictionary<string, string>? filters = null,
            [FromQuery] Dictionary<string, SortDirection>? orderBy = null)
        {
            _logger.LogInformation(
                "Received GET request to retrieve clubs for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}", 
                page, pageSize, filters?.Count, orderBy?.First().Key);
            var result = await _clubService.GetAllWithSpecificationAsync(
                pageNumber: page,
                pageSize: pageSize,
                filters: filters,
                orderBy: orderBy,
                cancellationToken: cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to retrieve clubs for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}. " +
                    "Error: {Error}",
                    page, pageSize, filters?.Count, orderBy?.First().Key, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully found {totalCount} clubs with filters: {countFilters}, orderBy: {orderBy}. " +
                "Returning page {page} with up to {pageSize} clubs", 
                result.Data.TotalCount, filters?.Count, orderBy?.First().Key, page, pageSize);

            return new PagedResponse<Club>()
            {
                CurrentPage = result.Data.CurrentPage,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount,
                Data = result.Data.Data.ToClubDtoEnumerable()
            };
        }

        // GET: api/v1/clubs/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.Clubs}.{Scopes.Read}")]
        [RequiredCapability(RoleCapabilityFeature.ManageClub, RoleCapabilityAction.Read)]
        [HttpGet("{id:guid}", Name = nameof(GetClubAsync))]
        public async Task<ActionResult<Club>> GetClubAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received GET request to retrieve club with ID: {Id}", id);
            var result = await _clubService.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to retrieve club with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully retrieved club with ID: {Id}", id);

            return result.Data.ToClubDto();            
        }

        // POST: api/v1/clubs
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.Clubs}.{Scopes.Write}")]
        [HttpPost]
        public async Task<ActionResult<Club>> PostClubAsync(Club club, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received POST request to CREATE club with name: {ClubName}, email: {ClubEmail}",
                club.Name, club.Email);
            var result = await _clubService.CreateAsync(club.ToClubEntity(), cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to create club with name: {ClubName}, email: {ClubEmail}. Error: {Error}",
                    club.Name, club.Email, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully created club with ID: {Id}, name: {ClubName}, email: {ClubEmail}",
                result.Data.Id, result.Data.Name, result.Data.Email);

            var apiVersion = HttpContext.GetRequestedApiVersion()?.ToString();
            return CreatedAtRoute(
                nameof(GetClubAsync),
                new { version = apiVersion, id = result.Data.Id.ToString() }, 
                result.Data.ToClubDto());
        }

        // PUT: api/v1/clubs/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.Clubs}.{Scopes.Write}")]
        [RequiredCapability(RoleCapabilityFeature.ManageClub, RoleCapabilityAction.ReadWrite)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Club>> PutClubAsync(Guid id, Club club, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received PUT request to UPDATE club with ID: {Id}", id);
            if (id != club.Id)
            {
                string errorMsg = $"Ids '{id}' and '{club.Id}' do not match.";
                _logger.LogWarning("Failed to update club with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _clubService.UpdateAsync(club.ToClubEntity(), cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update club with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully updated club with ID: {Id}", id);

            return result.Data.ToClubDto();
        }

        // PATCH: api/v1/clubs/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.Clubs}.{Scopes.Write}")]
        [RequiredCapability(RoleCapabilityFeature.ManageClub, RoleCapabilityAction.ReadWrite)]
        [HttpPatch("{id}")]
        public async Task<ActionResult<Club>> PatchClubAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received PATCH request to UPDATE {count} properties for club with ID: {Id}", 
                propertyUpdates.Count(), id);
            if (id == default)
            {
                string errorMsg = $"Id '{id}' is not valid.";
                _logger.LogWarning("Failed to update club with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _clubService.PatchAsync(id, propertyUpdates, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update club with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully updated club with ID: {Id}", id);

            return result.Data.ToClubDto();
        }

        // DELETE: api/v1/clubs/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.Clubs}.{Scopes.Delete}")]
        [RequiredSystemRole(SystemRole.SuperAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClubAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received DELETE request to delete club with ID: {Id}", id);
            var result = await _clubService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to delete club with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully deleted club with ID: {Id}", id);
            return NoContent();
        }
    }
}
