using MatchPoint.Api.Shared.Enums;
using MatchPoint.Api.Shared.Exceptions;
using MatchPoint.Api.Shared.Extensions;
using MatchPoint.Api.Shared.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;

namespace MatchPoint.ClubService.Services
{
    public class ClubManagementService(IClubRepository _clubRepository) : IClubManagementService
    {
        /// <inheritdoc />
        public Task<ClubEntity> GetByIdAsync(Guid id)
        {
            return _clubRepository.GetByIdAsync(id, trackChanges: false);
        }

        /// <inheritdoc />
        public Task<PagedResponse<ClubEntity>> GetAllWithSpecificationAsync(
            int pageNumber = 1,
            int pageSize = 500,
            Dictionary<string, object>? filters = null,
            KeyValuePair<string, SortDirection>? orderBy = null)
        {
            return _clubRepository.GetAllWithSpecificationAsync(
                pageNumber, pageSize, filters, orderBy, trackChanges: false);
        }

        /// <inheritdoc />
        public async Task<ClubEntity> CreateAsync(ClubEntity clubEntity)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            // Detect duplicate
            var filters = new Dictionary<string, object>() { { nameof(ClubEntity.Email), clubEntity.Email } };
            var existingClubs = await _clubRepository.CountAsync(filters);
            if (existingClubs > 0)
            {
                throw new DuplicateEntityException("A Club with the same email was found. Operation Canceled.");
            }

            // Set "Created" tracking fields
            clubEntity.SetTrackingFields();

            return await _clubRepository.CreateAsync(clubEntity);
        }

        /// <inheritdoc />
        public Task<ClubEntity> UpdateAsync(ClubEntity clubEntity)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            // Set "Modifed" tracking fields
            clubEntity.SetTrackingFields(updating: true);

            return _clubRepository.UpdateAsync(clubEntity);
        }

        /// <inheritdoc />
        public async Task<ClubEntity> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            // Find club
            var clubEntity = await _clubRepository.GetByIdAsync(id, trackChanges: false);

            // Update
            clubEntity.Patch(propertyUpdates);
            clubEntity.SetTrackingFields(updating: true);

            return await _clubRepository.UpdateAsync(clubEntity);
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync(Guid id)
        {
            return _clubRepository.DeleteAsync(id);
        }
    }
}
