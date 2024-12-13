using Asp.Versioning;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace MatchPoint.ClubService.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion(1)]
    public class ClubsController(IClubManagementService _clubService, ILogger<ClubsController> _logger) : ControllerBase
    {
        // GET: api/v1/clubs
        [MapToApiVersion(1)]
        [HttpGet]
        public async Task<ActionResult<PagedResponse<Club>>> GetClubsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = Constants.MaxPageSizeAllowed,
            [FromQuery] Dictionary<string, object>? filters = null,
            [FromQuery] Dictionary<string, SortDirection>? orderBy = null)
        {
            _logger.LogInformation(
                "Received GET request to retrieve clubs for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}", 
                page, pageSize, filters?.Count, orderBy?.First().Key);
            var result = await _clubService.GetAllWithSpecificationAsync(
                pageNumber: page,
                pageSize: pageSize,
                filters: filters,
                orderBy: orderBy);
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
                "Returning page {page} with {pageSize} clubs", 
                result.Data.TotalCount, filters?.Count, orderBy?.First().Key, page, pageSize);

            return new PagedResponse<Club>()
            {
                CurrentPage = result.Data.CurrentPage,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount,
                Data = result.Data.Data.ToClubDtoEnumerable()
            };
        }

        // GET: api/v1/clubs/5
        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Club>> GetClubAsync(Guid id)
        {
            _logger.LogInformation("Received GET request to retrieve club with ID: {Id}", id);
            var result = await _clubService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to retrieve club with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully retrieved club with ID: {Id}", id);

            return result.Data.ToClubDto();            
        }

        // PUT: api/v1/clubs/5
        [MapToApiVersion(1)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Club>> PutClubAsync(Guid id, Club club)
        {
            _logger.LogInformation("Received PUT request to UPDATE club with ID: {Id}", id);
            if (id != club.Id)
            {
                string errorMsg = $"Ids '{id}' and '{club.Id}' do not match.";
                _logger.LogWarning("Failed to update club with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _clubService.UpdateAsync(club.ToClubEntity());
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update club with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully updated club with ID: {Id}", id);

            return result.Data.ToClubDto();
        }

        // POST: api/v1/clubs
        [MapToApiVersion(1)]
        [HttpPost]
        public async Task<ActionResult<Club>> PostClubAsync(Club club)
        {
            _logger.LogInformation(
                "Received POST request to CREATE club with name: {clubName}, email: {clubEmail}", 
                club.Name, club.Email);
            var result = await _clubService.CreateAsync(club.ToClubEntity());
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to create club with name: {clubName}, email: {clubEmail}. Error: {Error}", 
                    club.Name, club.Email, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully created club with ID: {Id}, name: {clubName}, email: {clubEmail}", 
                result.Data.Id, result.Data.Name, result.Data.Email);

            return CreatedAtAction(nameof(GetClubAsync), new { id = club.Id }, club);
        }

        // DELETE: api/v1/clubs/5
        [MapToApiVersion(1)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClubAsync(Guid id)
        {
            _logger.LogInformation("Received DELETE request to delete club with ID: {Id}", id);
            var result = await _clubService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete club with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully deleted club with ID: {Id}", id);
            return NoContent();
        }
    }
}
