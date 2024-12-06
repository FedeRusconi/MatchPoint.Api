using MatchPoint.Api.Shared.Enums;
using MatchPoint.Api.Shared.Extensions;
using MatchPoint.Api.Shared.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;

namespace MatchPoint.ClubService.Services
{
    public class ClubManagementService(IClubRepository _clubRepository) : IClubManagementService
    {
        public Task<ClubEntity> CreateAsync(ClubEntity clubEntity)
        {
            throw new NotImplementedException();
        }

        public Task<ClubEntity> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<ClubEntity>> GetAllWithSpecificationAsync(int pageNumber = 1, int pageSize = 500, Dictionary<string, object>? filters = null, KeyValuePair<string, SortDirection>? orderBy = null)
        {
            throw new NotImplementedException();
        }

        public Task<ClubEntity> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
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

        public Task<ClubEntity> UpdateAsync(ClubEntity clubEntity)
        {
            throw new NotImplementedException();
        }
    }
}
