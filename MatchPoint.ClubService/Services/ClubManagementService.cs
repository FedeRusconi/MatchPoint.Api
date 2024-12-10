using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;

namespace MatchPoint.ClubService.Services
{
    public class ClubManagementService(IClubRepository _clubRepository, ILogger _logger) : IClubManagementService
    {
        /// <inheritdoc />
        public async Task<IServiceResult<ClubEntity>> GetByIdAsync(Guid id)
        {
            _logger.LogDebug("Attempting to retrieve club with ID: {Id}", id);

            var clubEntity = await _clubRepository.GetByIdAsync(id, trackChanges: false);
            if (clubEntity == null)
            {
                _logger.LogWarning("Not Found: Club with ID: {Id} not found", id);
                return ServiceResult<ClubEntity>.Failure(
                    $"Club with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            _logger.LogDebug("Club with ID: {Id} found successfully", id);
            return ServiceResult<ClubEntity>.Success(clubEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<PagedResponse<ClubEntity>>> GetAllWithSpecificationAsync(
            int pageNumber = 1,
            int pageSize = Constants.MaxPageSizeAllowed,
            Dictionary<string, object>? filters = null,
            KeyValuePair<string, SortDirection>? orderBy = null)
        {
            // Validate params
            var paginationValidation = QuerySpecificationHelpers.ValidatePagination<ClubEntity>(pageNumber, pageSize);
            if (paginationValidation != null) return paginationValidation;

            var orderByValidation = QuerySpecificationHelpers.ValidateOrderBy<ClubEntity>(orderBy);
            if (orderByValidation != null) return orderByValidation;

            var filtersValidation = QuerySpecificationHelpers.ValidateFilters<ClubEntity>(filters);
            if (filtersValidation != null) return filtersValidation;

            _logger.LogDebug(
                "Attepting to retrieve clubs with {Count} filters", filters != null ? filters.Count : "no");

            var clubs = await _clubRepository.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, filters, orderBy, trackChanges: false);

            _logger.LogDebug("Receieved {PageSize} of {Count} Clubs found.", pageSize, clubs.TotalCount);

            return ServiceResult<PagedResponse<ClubEntity>>.Success(clubs);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubEntity>> CreateAsync(ClubEntity clubEntity)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            _logger.LogDebug("Attempting to create club with email: {Email}", clubEntity.Email);

            // Detect duplicate
            var filters = new Dictionary<string, object>() { { nameof(ClubEntity.Email), clubEntity.Email } };
            var existingClubs = await _clubRepository.CountAsync(filters);
            if (existingClubs > 0)
            {
                _logger.LogWarning("Conflict: Club with email '{Email}' already exists.", clubEntity.Email);
                return ServiceResult<ClubEntity>.Failure(
                    "A Club with the same email was found. Operation Canceled.", ServiceResultType.Conflict);
            }

            // Set "Created" tracking fields
            clubEntity.SetTrackingFields();

            var createdEntity = await _clubRepository.CreateAsync(clubEntity);
            _logger.LogDebug(
                "Club with email '{Email}' created successfully. Id: {Id}", createdEntity.Email, createdEntity.Id);
            return ServiceResult<ClubEntity>.Success(createdEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubEntity>> UpdateAsync(ClubEntity clubEntity)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            _logger.LogDebug("Attempting to update club with Id: {Id}", clubEntity.Id);

            // Set "Modifed" tracking fields
            clubEntity.SetTrackingFields(updating: true);

            var updatedEntity = await _clubRepository.UpdateAsync(clubEntity);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Club with Id '{Id}' not found.", clubEntity.Id);
                return ServiceResult<ClubEntity>.Failure(
                    $"Club with id '{clubEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Club with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<ClubEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubEntity>> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            _logger.LogDebug("Attempting to patch club with Id: {Id}", id);

            // Find club
            var clubEntity = await _clubRepository.GetByIdAsync(id, trackChanges: false);
            if (clubEntity == null)
            {
                _logger.LogWarning("Not Found: Club with Id '{Id}' not found.", id);
                return ServiceResult<ClubEntity>.Failure(
                    $"Club with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            // Update
            clubEntity.Patch(propertyUpdates);
            clubEntity.SetTrackingFields(updating: true);

            var updatedEntity = await _clubRepository.UpdateAsync(clubEntity);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Club with Id '{Id}' not found.", clubEntity.Id);
                return ServiceResult<ClubEntity>.Failure(
                    $"Club with id '{clubEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Club with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<ClubEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubEntity>> DeleteAsync(Guid id)
        {
            _logger.LogDebug("Attempting to delete club with Id: {Id}", id);
            var deletedEntity = await _clubRepository.DeleteAsync(id);

            if (deletedEntity == null)
            {
                _logger.LogWarning("Not Found: Club with Id '{Id}' not found.", id);
                return ServiceResult<ClubEntity>.Failure(
                    $"Club with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Club with Id '{Id}' deleted successfully.", deletedEntity.Id);
            return ServiceResult<ClubEntity>.Success(deletedEntity);
        }
    }
}
