using Asp.Versioning;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace MatchPoint.ClubService.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/clubs/{clubId:guid}/staff")]
    [ApiController]
    public class ClubStaffController(IClubStaffService _clubStaffService, ILogger<ClubStaffController> _logger) 
        : ControllerBase
    {
        // GET: api/v1/clubs/[guid]/staff
        [MapToApiVersion(1)]
        [RequiredScope("Clubs.Read")]
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ClubStaff>>> GetClubStaffAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = Constants.MaxPageSizeAllowed,
            [FromQuery] Dictionary<string, string>? filters = null,
            [FromQuery] Dictionary<string, SortDirection>? orderBy = null)
        {
            _logger.LogInformation(
                "Received GET request to retrieve club staff for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}",
                page, pageSize, filters?.Count, orderBy?.First().Key);
            var result = await _clubStaffService.GetAllWithSpecificationAsync(
                pageNumber: page,
                pageSize: pageSize,
                filters: filters,
                orderBy: orderBy);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to retrieve club staff for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}. " +
                    "Error: {Error}",
                    page, pageSize, filters?.Count, orderBy?.First().Key, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully found {totalCount} club staff with filters: {countFilters}, orderBy: {orderBy}. " +
                "Returning page {page} with up to {pageSize} club staff",
                result.Data.TotalCount, filters?.Count, orderBy?.First().Key, page, pageSize);

            return new PagedResponse<ClubStaff>()
            {
                CurrentPage = result.Data.CurrentPage,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount,
                Data = result.Data.Data.ToClubStaffDtoEnumerable()
            };
        }

        // GET: api/v1/clubs/[guid]/staff/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("Clubs.Read")]
        [HttpGet("{id:guid}", Name = nameof(GetClubStaffAsync))]
        public async Task<ActionResult<ClubStaff>> GetClubStaffAsync(Guid id)
        {
            _logger.LogInformation("Received GET request to retrieve club staff with ID: {Id}", id);
            var result = await _clubStaffService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to retrieve club staff with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully retrieved club staff with ID: {Id}", id);

            return result.Data.ToClubStaffDto();
        }

        // POST: api/v1/clubs/[guid]/staff
        [MapToApiVersion(1)]
        [RequiredScope("Clubs.Write")]
        [HttpPost]
        public async Task<ActionResult<Club>> PostClubStaffAsync(ClubStaff clubStaff)
        {
            _logger.LogInformation(
                "Received POST request to CREATE club staff with name: {clubStaffName}, email: {clubStaffEmail}",
                clubStaff.FullName, clubStaff.Email);
            var result = await _clubStaffService.CreateAsync(clubStaff.ToClubStaffEntity());
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to create club staff with name: {clubStaffName}, email: {clubStaffEmail}. Error: {Error}",
                    clubStaff.FullName, clubStaff.Email, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully created club staff with ID: {Id}, email: {clubStaffEmail}",
                result.Data.Id, result.Data.Email);

            var apiVersion = HttpContext.GetRequestedApiVersion()?.ToString();
            return CreatedAtRoute(
                nameof(GetClubStaffAsync),
                new { version = apiVersion, id = result.Data.Id.ToString() },
                result.Data.ToClubStaffDto());
        }

        // PATCH: api/v1/clubs/[guid]/staff/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("Clubs.Write")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ClubStaff>> PatchClubStaffAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates)
        {
            _logger.LogInformation(
                "Received PATCH request to UPDATE {count} properties for club staff with ID: {Id}",
                propertyUpdates.Count(), id);
            if (id == default)
            {
                string errorMsg = $"Id '{id}' is not valid.";
                _logger.LogWarning("Failed to update club staff with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _clubStaffService.PatchAsync(id, propertyUpdates);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update club staff with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully updated club staff with ID: {Id}", id);

            return result.Data.ToClubStaffDto();
        }

        // DELETE: api/v1/clubs/[guid]/staff/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("Clubs.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClubStaffAsync(Guid id)
        {
            _logger.LogInformation("Received DELETE request to delete club staff with ID: {Id}", id);
            var result = await _clubStaffService.DeleteAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to delete club staff with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully deleted club staff with ID: {Id}", id);
            return NoContent();
        }
    }
}
