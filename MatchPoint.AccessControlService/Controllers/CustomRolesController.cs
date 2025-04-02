using Asp.Versioning;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Attributes;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace MatchPoint.AccessControlService.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomRolesController(ICustomRoleService _customRoleService, ILogger<CustomRolesController> _logger)
        : ControllerBase
    {
        // GET: api/v1/customRoles
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.CustomRoles}.{Scopes.Read}")]
        [HttpGet]
        public async Task<ActionResult<PagedResponse<CustomRole>>> GetCustomRolesAsync(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = Constants.MaxPageSizeAllowed,
            [FromQuery] Dictionary<string, string>? filters = null,
            [FromQuery] Dictionary<string, SortDirection>? orderBy = null)
        {
            _logger.LogInformation(
                "Received GET request to retrieve custom roles for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}",
                page, pageSize, filters?.Count, orderBy?.First().Key);
            var result = await _customRoleService.GetAllWithSpecificationAsync(
                pageNumber: page,
                pageSize: pageSize,
                filters: filters,
                orderBy: orderBy,
                cancellationToken: cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to retrieve custom roles for page: {page}, page size: {pageSize}, filters: {countFilters}, orderBy: {orderBy}. " +
                    "Error: {Error}",
                    page, pageSize, filters?.Count, orderBy?.First().Key, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully found {totalCount} custom roles with filters: {countFilters}, orderBy: {orderBy}. " +
                "Returning page {page} with up to {pageSize} custom roles",
                result.Data.TotalCount, filters?.Count, orderBy?.First().Key, page, pageSize);

            return new PagedResponse<CustomRole>()
            {
                CurrentPage = result.Data.CurrentPage,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount,
                Data = result.Data.Data.ToCustomRoleDtoEnumerable()
            };
        }

        // GET: api/v1/customRoles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.CustomRoles}.{Scopes.Read}")]
        [HttpGet("{id:guid}", Name = nameof(GetCustomRoleAsync))]
        public async Task<ActionResult<CustomRole>> GetCustomRoleAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received GET request to retrieve custom role with ID: {Id}", id);
            var result = await _customRoleService.GetByIdAsync(id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to retrieve custom role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully retrieved custom role with ID: {Id}", id);

            return result.Data.ToCustomRoleDto();
        }

        // POST: api/v1/customRoles
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.CustomRoles}.{Scopes.Write}")]
        [RequiredSystemRole(SystemRole.SuperAdmin)]
        [HttpPost]
        public async Task<ActionResult<CustomRole>> PostCustomRoleAsync(CustomRole customRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received POST request to CREATE custom role with name: {RoleName}", customRole.Name);
            var result = await _customRoleService.CreateAsync(customRole.ToCustomRoleEntity(), cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to create custom role with name: {RoleName}. Error: {Error}",
                    customRole.Name, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully created custom role with ID: {Id}, name: {RoleName}",
                result.Data.Id, result.Data.Name);

            var apiVersion = HttpContext.GetRequestedApiVersion()?.ToString();
            return CreatedAtRoute(
                nameof(GetCustomRoleAsync),
                new { version = apiVersion, id = result.Data.Id.ToString() },
                result.Data.ToCustomRoleDto());
        }

        // PUT: api/v1/customRoles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.CustomRoles}.{Scopes.Write}")]
        [RequiredSystemRole(SystemRole.SuperAdmin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomRole>> PutCustomRoleAsync(Guid id, CustomRole customRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received PUT request to UPDATE custom role with ID: {Id}", id);
            if (id != customRole.Id)
            {
                string errorMsg = $"Ids '{id}' and '{customRole.Id}' do not match.";
                _logger.LogWarning("Failed to update custom role with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _customRoleService.UpdateAsync(customRole.ToCustomRoleEntity(), cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update custom role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully updated custom role with ID: {Id}", id);

            return result.Data.ToCustomRoleDto();
        }

        // PATCH: api/v1/customRoles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.CustomRoles}.{Scopes.Write}")]
        [RequiredSystemRole(SystemRole.SuperAdmin)]
        [HttpPatch("{id}")]
        public async Task<ActionResult<CustomRole>> PatchCustomRoleAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received PATCH request to UPDATE {count} properties for custom role with ID: {Id}",
                propertyUpdates.Count(), id);
            if (id == default)
            {
                string errorMsg = $"Id '{id}' is not valid.";
                _logger.LogWarning("Failed to update custom role with ID: {Id}. Error: {Error}", id, errorMsg);
                return BadRequest(errorMsg);
            }

            var result = await _customRoleService.PatchAsync(id, propertyUpdates, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to update custom role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully updated custom role with ID: {Id}", id);

            return result.Data.ToCustomRoleDto();
        }

        // DELETE: api/v1/customRoles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope($"{Scopes.CustomRoles}.{Scopes.Delete}")]
        [RequiredSystemRole(SystemRole.SuperAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomRoleAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received DELETE request to delete custom role with ID: {Id}", id);
            var result = await _customRoleService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Failed to delete custom role with ID: {Id}. Error: {Error}", id, result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation("Successfully deleted custom role with ID: {Id}", id);
            return NoContent();
        }
    }
}
