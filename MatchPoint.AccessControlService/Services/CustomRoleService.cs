using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ServiceDefaults;

namespace MatchPoint.AccessControlService.Services
{
    public class CustomRoleService(
        ICustomRoleRepository _customRoleRepository,
        ISessionService _sessionService,
        ILogger<CustomRoleService> _logger) : ICustomRoleService
    {
        /// <inheritdoc />
        public async Task<IServiceResult<CustomRoleEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve custom role with ID: {Id}", id);

            var customRoleEntity = await _customRoleRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            if (customRoleEntity == null)
            {
                _logger.LogWarning("Not Found: Custom role with ID: {Id} not found", id);
                return ServiceResult<CustomRoleEntity>.Failure(
                    $"Custom role with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            _logger.LogDebug("Custom role with ID: {Id} found successfully", id);
            return ServiceResult<CustomRoleEntity>.Success(customRoleEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<PagedResponse<CustomRoleEntity>>> GetAllWithSpecificationAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null)
        {
            // Validate params
            var paginationValidation = QuerySpecificationHelpers.ValidatePagination<CustomRoleEntity>(pageNumber, pageSize);
            if (paginationValidation != null) return paginationValidation;

            var orderByValidation = QuerySpecificationHelpers.ValidateOrderBy<CustomRoleEntity>(orderBy);
            if (orderByValidation != null) return orderByValidation;

            var filtersValidation = QuerySpecificationHelpers.ValidateFilters<CustomRoleEntity>(filters);
            if (filtersValidation != null) return filtersValidation;

            _logger.LogDebug(
                "Attempting to retrieve custom roles with {Count} filters", filters != null ? filters.Count : "no");

            var customRoles = await _customRoleRepository.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, cancellationToken, filters, orderBy, trackChanges: false);

            _logger.LogDebug("Received {PageSize} of {Count} Custom roles found.", customRoles.Data.Count(), customRoles.TotalCount);

            return ServiceResult<PagedResponse<CustomRoleEntity>>.Success(customRoles);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<CustomRoleEntity>> CreateAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(customRoleEntity);

            _logger.LogDebug("Attempting to create custom role with name: {Name}", customRoleEntity.Name);

            // Detect duplicate
            var filters = new Dictionary<string, string>() { { nameof(CustomRoleEntity.Name), customRoleEntity.Name } };
            var existingCustomRoles = await _customRoleRepository.CountAsync(cancellationToken, filters);
            if (existingCustomRoles > 0)
            {
                _logger.LogWarning("Conflict: Custom role with name '{Name}' already exists.", customRoleEntity.Name);
                return ServiceResult<CustomRoleEntity>.Failure(
                    "A custom role with the same name was found. Operation Canceled.", ServiceResultType.Conflict);
            }

            // Set "Created" tracking fields
            customRoleEntity.SetTrackingFields(_sessionService.CurrentUserId);

            var createdEntity = await _customRoleRepository.CreateAsync(customRoleEntity, cancellationToken);
            if (createdEntity == null)
            {
                _logger.LogWarning("Conflict: Custom role with Id '{Id}' already exists.", customRoleEntity.Id);
                return ServiceResult<CustomRoleEntity>.Failure(
                    $"Custom role with id '{customRoleEntity.Id}' already exists.", ServiceResultType.Conflict);
            }

            _logger.LogDebug(
                "Custom role with name '{Name}' created successfully. Id: {Id}", createdEntity.Name, createdEntity.Id);
            return ServiceResult<CustomRoleEntity>.Success(createdEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<CustomRoleEntity>> UpdateAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(customRoleEntity);

            _logger.LogDebug("Attempting to update custom role with Id: {Id}", customRoleEntity.Id);

            // Set "Modifed" tracking fields
            customRoleEntity.SetTrackingFields(_sessionService.CurrentUserId, updating: true);

            var updatedEntity = await _customRoleRepository.UpdateAsync(customRoleEntity, cancellationToken);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Custom role with Id '{Id}' not found.", customRoleEntity.Id);
                return ServiceResult<CustomRoleEntity>.Failure(
                    $"Custom role with id '{customRoleEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Custom role with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<CustomRoleEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<CustomRoleEntity>> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            _logger.LogDebug("Attempting to patch custom role with Id: {Id}", id);

            // Find custom role
            var customRoleEntity = await _customRoleRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            if (customRoleEntity == null)
            {
                _logger.LogWarning("Not Found: Custom role with Id '{Id}' not found.", id);
                return ServiceResult<CustomRoleEntity>.Failure(
                    $"Custom role with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            // Update
            try
            {
                customRoleEntity.Patch(propertyUpdates);
                customRoleEntity.SetTrackingFields(_sessionService.CurrentUserId, updating: true);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("{msg}", ex.Message);
                return ServiceResult<CustomRoleEntity>.Failure(ex.Message, ServiceResultType.BadRequest);
            }

            var updatedEntity = await _customRoleRepository.UpdateAsync(customRoleEntity, cancellationToken);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Custom role with Id '{Id}' not found.", customRoleEntity.Id);
                return ServiceResult<CustomRoleEntity>.Failure(
                    $"Custom role with id '{customRoleEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Custom role with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<CustomRoleEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<CustomRoleEntity>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var customRoleEntity = await _customRoleRepository.GetByIdAsync(id, cancellationToken);
            if (customRoleEntity == null)
            {
                _logger.LogWarning("Not Found: Custom role with Id '{Id}' not found.", id);
                return ServiceResult<CustomRoleEntity>.Failure(
                    $"Custom role with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug("Attempting to delete custom role with Id: {Id}", id);
            var deletedEntity = await _customRoleRepository.DeleteAsync(customRoleEntity, cancellationToken);
            if (deletedEntity == null)
            {
                _logger.LogWarning("Not Found: Custom role with Id '{Id}' not found.", id);
                return ServiceResult<CustomRoleEntity>.Failure(
                    $"Custom role with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Custom role with Id '{Id}' deleted successfully.", deletedEntity.Id);
            return ServiceResult<CustomRoleEntity>.Success(deletedEntity);
        }
    }
}
