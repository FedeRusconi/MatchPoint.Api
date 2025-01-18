using Asp.Versioning;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
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
        [RequiredScope("CustomRoles.Read")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomRole>>> GetCustomRolesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received GET request to retrieve all custom roles");
            var result = await _customRoleService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning(
                    "Failed to retrieve custom roles. Error: {Error}", result.Error);
                return result.ToFailureActionResult(this);
            }
            _logger.LogInformation(
                "Successfully found {totalCount} custom roles", result.Data.Count());

            return result.Data.ToCustomRoleDtoEnumerable().ToList();
        }

        // GET: api/v1/customRoles/[guid]
        [MapToApiVersion(1)]
        [RequiredScope("CustomRoles.Read")]
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
        [RequiredScope("CustomRoles.Write")]
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
        [RequiredScope("CustomRoles.Write")]
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
        [RequiredScope("CustomRoles.Write")]
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
        [RequiredScope("CustomRoles.Delete")]
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
