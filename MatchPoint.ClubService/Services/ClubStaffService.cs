using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;

namespace MatchPoint.ClubService.Services
{
    public class ClubStaffService(
        IClubStaffRepository _clubStaffRepository, IAzureAdService _azureAdService, ILogger<ClubStaffService> _logger) : IClubStaffService
    {
        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> GetByIdAsync(Guid id)
        {
            _logger.LogDebug("Attempting to retrieve club staff with ID: {Id}", id);

            // Run tasks for AzureAD and DB
            var azureAdTask = _azureAdService.GetUserByIdAsync(id);
            var dbTask = _clubStaffRepository.GetByIdAsync(id, trackChanges: false);

            await Task.WhenAll(azureAdTask, dbTask);

            var clubStaffAzureAd = azureAdTask.Result;
            var clubStaffEntity = dbTask.Result;
            if (clubStaffAzureAd == null || clubStaffEntity == null)
            {
                _logger.LogWarning("Not Found: Club staff with ID: {Id} not found", id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            // Add Azure AD properties to entity
            clubStaffEntity.SetAzureAdProperties(clubStaffAzureAd);

            _logger.LogDebug("Club staff with ID: {Id} found successfully", id);
            return ServiceResult<ClubStaffEntity>.Success(clubStaffEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<PagedResponse<ClubStaffEntity>>> GetAllWithSpecificationAsync(
            int pageNumber,
            int pageSize,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null)
        {
            // Validate params
            var paginationValidation = QuerySpecificationHelpers.ValidatePagination<ClubStaffEntity>(pageNumber, pageSize);
            if (paginationValidation != null) return paginationValidation;

            var orderByValidation = QuerySpecificationHelpers.ValidateOrderBy<ClubStaffEntity>(orderBy);
            if (orderByValidation != null) return orderByValidation;

            var filtersValidation = QuerySpecificationHelpers.ValidateFilters<ClubStaffEntity>(filters);
            if (filtersValidation != null) return filtersValidation;

            _logger.LogDebug(
                "Attepting to retrieve club staff with {Count} filters", filters != null ? filters.Count : "no");

            var clubStaff = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, filters, orderBy, trackChanges: false);

            _logger.LogDebug("Receieved {PageSize} of {Count} Club staff found.", clubStaff.Data.Count(), clubStaff.TotalCount);

            return ServiceResult<PagedResponse<ClubStaffEntity>>.Success(clubStaff);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> CreateAsync(ClubStaffEntity clubStaffEntity)
        {
            ArgumentNullException.ThrowIfNull(clubStaffEntity);

            _logger.LogDebug("Attempting to create club staff with Email: {Email}", clubStaffEntity.Email);

            // TODO: In addition to props from ClubStaff, set DisplayName and CompanyName (clubName)

            // Detect duplicate
            //var filters = new Dictionary<string, string>() { { nameof(ClubEntity.Email), clubStaffEntity.Email } };
            //var existingClubs = await _clubStaffRepository.CountAsync(filters);
            //if (existingClubs > 0)
            //{
            //    _logger.LogWarning("Conflict: Club staff with email '{Email}' already exists.", clubStaffEntity.Email);
            //    return ServiceResult<ClubStaffEntity>.Failure(
            //        "A Club staff with the same email was found. Operation Canceled.", ServiceResultType.Conflict);
            //}

            // Set "Created" tracking fields
            clubStaffEntity.SetTrackingFields();

            var createdEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity);
            if (createdEntity == null)
            {
                _logger.LogWarning("Conflict: Club staff with Id '{Id}' already exists.", clubStaffEntity.Id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{clubStaffEntity.Id}' already exists.", ServiceResultType.Conflict);
            }

            _logger.LogDebug(
                "Club staff with email '{Email}' created successfully. Id: {Id}", createdEntity.Email, createdEntity.Id);
            return ServiceResult<ClubStaffEntity>.Success(createdEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> UpdateAsync(ClubStaffEntity clubStaffEntity)
        {
            ArgumentNullException.ThrowIfNull(clubStaffEntity);

            _logger.LogDebug("Attempting to update club staff with Id: {Id}", clubStaffEntity.Id);

            // Set "Modifed" tracking fields
            clubStaffEntity.SetTrackingFields(updating: true);

            var updatedEntity = await _clubStaffRepository.UpdateAsync(clubStaffEntity);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", clubStaffEntity.Id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{clubStaffEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Club staff with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<ClubStaffEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            _logger.LogDebug("Attempting to patch club staff with Id: {Id}", id);

            // Find club
            var clubStaffEntity = await _clubStaffRepository.GetByIdAsync(id, trackChanges: false);
            if (clubStaffEntity == null)
            {
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            // Update
            try
            {
                clubStaffEntity.Patch(propertyUpdates);
                clubStaffEntity.SetTrackingFields(updating: true);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("{msg}", ex.Message);
                return ServiceResult<ClubStaffEntity>.Failure(ex.Message, ServiceResultType.BadRequest);
            }

            var updatedEntity = await _clubStaffRepository.UpdateAsync(clubStaffEntity);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", clubStaffEntity.Id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{clubStaffEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Club staff with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<ClubStaffEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> DeleteAsync(Guid id)
        {
            _logger.LogDebug("Attempting to delete club staff with Id: {Id}", id);
            var deletedEntity = await _clubStaffRepository.DeleteAsync(id);

            if (deletedEntity == null)
            {
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Club staff with Id '{Id}' deleted successfully.", deletedEntity.Id);
            return ServiceResult<ClubStaffEntity>.Success(deletedEntity);
        }
    }
}
