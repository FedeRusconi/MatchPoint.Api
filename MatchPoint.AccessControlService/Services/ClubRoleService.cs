using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ServiceDefaults;
using MatchPoint.ServiceDefaults.MockEventBus;

namespace MatchPoint.AccessControlService.Services
{
    public class ClubRoleService(
        IClubRoleRepository _clubRoleRepository,
        ISessionService _sessionService,
        IEventBusClient _eventBus,
        ILogger<ClubRoleService> _logger) : IClubRoleService
    {
        /// <inheritdoc />
        public async Task<IServiceResult<ClubRoleEntity>> GetByIdAsync(Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve club role with ID: {Id}", id);

            var clubRoleEntity = await _clubRoleRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            if (clubRoleEntity == null)
            {
                _logger.LogWarning("Not Found: Club role with ID: {Id} not found", id);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"Club role with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (clubRoleEntity.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of club role with ID: {Id} does not match provided '{clubId}'",
                    id,
                    clubId);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"ClubId of club role with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }
            _logger.LogDebug("Club role with ID: {Id} found successfully", id);
            return ServiceResult<ClubRoleEntity>.Success(clubRoleEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<PagedResponse<ClubRoleEntity>>> GetAllWithSpecificationAsync(
            Guid clubId, 
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken, 
            Dictionary<string, string>? filters = null, 
            Dictionary<string, SortDirection>? orderBy = null)
        {
            // Validate params
            var paginationValidation = QuerySpecificationHelpers.ValidatePagination<ClubRoleEntity>(pageNumber, pageSize);
            if (paginationValidation != null) return paginationValidation;

            var orderByValidation = QuerySpecificationHelpers.ValidateOrderBy<ClubRoleEntity>(orderBy);
            if (orderByValidation != null) return orderByValidation;

            var filtersValidation = QuerySpecificationHelpers.ValidateFilters<ClubRoleEntity>(filters);
            if (filtersValidation != null) return filtersValidation;

            _logger.LogDebug(
                "Attempting to retrieve club roles with {Count} filters", filters != null ? filters.Count : "no");

            // Filter for club id is added automatically
            filters ??= [];
            filters.Add(nameof(ClubRoleEntity.ClubId), clubId.ToString());
            var clubRoles = await _clubRoleRepository.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, cancellationToken, filters, orderBy, trackChanges: false);

            _logger.LogDebug("Received {PageSize} of {Count} Club roles found.", clubRoles.Data.Count(), clubRoles.TotalCount);

            return ServiceResult<PagedResponse<ClubRoleEntity>>.Success(clubRoles);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubRoleEntity>> CreateAsync(
            Guid clubId, ClubRoleEntity clubRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubRoleEntity);

            _logger.LogDebug("Attempting to create club role with name: {Name}", clubRoleEntity.Name);

            // Detect duplicate
            var filters = new Dictionary<string, string>() { { nameof(ClubRoleEntity.Name), clubRoleEntity.Name } };
            var existingClubRoles = await _clubRoleRepository.CountAsync(cancellationToken, filters);
            if (existingClubRoles > 0)
            {
                _logger.LogWarning("Conflict: Club role with name '{Name}' already exists.", clubRoleEntity.Name);
                return ServiceResult<ClubRoleEntity>.Failure(
                    "A club role with the same name was found. Operation Canceled.", ServiceResultType.Conflict);
            }

            // Set ClubId and "Created" tracking fields
            clubRoleEntity.ClubId = clubId;
            clubRoleEntity.SetTrackingFields(_sessionService.CurrentUserId);

            var createdEntity = await _clubRoleRepository.CreateAsync(clubRoleEntity, cancellationToken);
            if (createdEntity == null)
            {
                _logger.LogWarning("Conflict: Club role with Id '{Id}' already exists.", clubRoleEntity.Id);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"Club role with id '{clubRoleEntity.Id}' already exists.", ServiceResultType.Conflict);
            }

            _logger.LogDebug(
                "Club role with name '{Name}' created successfully. Id: {Id}", createdEntity.Name, createdEntity.Id);
            return ServiceResult<ClubRoleEntity>.Success(createdEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubRoleEntity>> UpdateAsync(
            Guid clubId, ClubRoleEntity clubRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubRoleEntity);

            _logger.LogDebug("Attempting to update club role with Id: {Id}", clubRoleEntity.Id);

            // Set ClubId and "Modifed" tracking fields
            clubRoleEntity.ClubId = clubId;
            clubRoleEntity.SetTrackingFields(_sessionService.CurrentUserId, updating: true);

            var updatedEntity = await _clubRoleRepository.UpdateAsync(clubRoleEntity, cancellationToken);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Club role with Id '{Id}' not found.", clubRoleEntity.Id);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"Club role with id '{clubRoleEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            // Send an event to notify any listener
            await _eventBus.PublishAsync(Topics.ClubRoles, EventType.Update, clubRoleEntity.Id);

            _logger.LogDebug(
                "Club role with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<ClubRoleEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubRoleEntity>> PatchAsync(
            Guid clubId, Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            _logger.LogDebug("Attempting to patch club role with Id: {Id}", id);

            // Find club
            var clubRoleEntity = await _clubRoleRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            if (clubRoleEntity == null)
            {
                _logger.LogWarning("Not Found: Club role with Id '{Id}' not found.", id);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"Club role with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (clubRoleEntity.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of club role with ID: {Id} does not match provided '{ClubId}'",
                    id,
                    clubId);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"ClubId of club role with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }

            // Update
            try
            {
                clubRoleEntity.Patch(propertyUpdates);
                clubRoleEntity.SetTrackingFields(_sessionService.CurrentUserId, updating: true);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("{msg}", ex.Message);
                return ServiceResult<ClubRoleEntity>.Failure(ex.Message, ServiceResultType.BadRequest);
            }

            var updatedEntity = await _clubRoleRepository.UpdateAsync(clubRoleEntity, cancellationToken);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Club role with Id '{Id}' not found.", clubRoleEntity.Id);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"Club role with id '{clubRoleEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            // Send an event to notify any listener
            await _eventBus.PublishAsync(Topics.ClubRoles, EventType.Update, clubRoleEntity.Id);

            _logger.LogDebug(
                "Club role with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<ClubRoleEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubRoleEntity>> DeleteAsync(
            Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to delete club role with Id: {Id}", id);

            var clubRoleEntity = await _clubRoleRepository.GetByIdAsync(id, cancellationToken);
            if (clubRoleEntity == null)
            {
                _logger.LogWarning("Not Found: Club role with Id '{Id}' not found.", id);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"Club role with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (clubRoleEntity.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of club role with ID: {Id} does not match provided '{ClubId}'",
                    id,
                    clubId);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"ClubId of club role with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }

            var deletedEntity = await _clubRoleRepository.DeleteAsync(clubRoleEntity, cancellationToken);
            if (deletedEntity == null)
            {
                _logger.LogWarning("Not Found: Club role with Id '{Id}' not found.", id);
                return ServiceResult<ClubRoleEntity>.Failure(
                    $"Club role with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            // Send an event to notify any listener
            await _eventBus.PublishAsync(Topics.ClubRoles, EventType.Delete, clubRoleEntity.Id);

            _logger.LogDebug(
                "Club role with Id '{Id}' deleted successfully.", deletedEntity.Id);
            return ServiceResult<ClubRoleEntity>.Success(deletedEntity);
        }
    }
}
