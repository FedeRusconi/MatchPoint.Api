using Asp.Versioning;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MatchPoint.ClubService.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion(1)]
    public class ClubsController(IClubManagementService _clubService) : ControllerBase
    {

        // GET: api/v1/clubs
        [MapToApiVersion(1)]
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ClubEntity>>> GetClubs(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] Dictionary<string, object>? filters,
            [FromQuery] KeyValuePair<string, SortDirection>? orderBy)
        {
            var result = await _clubService.GetAllWithSpecificationAsync(
                pageNumber: page,
                pageSize: pageSize,
                filters: filters,
                orderBy: orderBy);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return result.ToFailureActionResult(this);
        }

        // GET: api/v1/clubs/5
        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ClubEntity>> GetClubEntity(Guid id)
        {
            var result = await _clubService.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                return result.ToFailureActionResult(this);
            }
            return Ok(result);            
        }

        // PUT: api/v1/clubs/5
        [MapToApiVersion(1)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ClubEntity>> PutClubEntity(Guid id, ClubEntity clubEntity)
        {
            if (id != clubEntity.Id)
            {
                return BadRequest($"Ids '{id}' and '{clubEntity.Id}' do not match.");
            }

            var result = await _clubService.UpdateAsync(clubEntity);
            if (!result.IsSuccess)
            {                
                return result.ToFailureActionResult(this);
            }

            return Ok(result);
        }

        // POST: api/v1/clubs
        [MapToApiVersion(1)]
        [HttpPost]
        public async Task<ActionResult<ClubEntity>> PostClubEntity(ClubEntity clubEntity)
        {
            var result = await _clubService.CreateAsync(clubEntity);
            if (!result.IsSuccess)
            {
                return result.ToFailureActionResult(this);
            }

            return CreatedAtAction("GetClubEntity", new { id = clubEntity.Id }, clubEntity);
        }

        // DELETE: api/v1/clubs/5
        [MapToApiVersion(1)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClubEntity(Guid id)
        {
            var result = await _clubService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return result.ToFailureActionResult(this);
            }

            return NoContent();
        }
    }
}
