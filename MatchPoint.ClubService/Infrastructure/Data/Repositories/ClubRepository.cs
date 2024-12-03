using MatchPoint.Api.Shared.Enums;
using MatchPoint.Api.Shared.Exceptions;
using MatchPoint.Api.Shared.Extensions;
using MatchPoint.Api.Shared.Models;
using MatchPoint.Api.Shared.Repositories;
using MatchPoint.Api.Shared.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Infrastructure.Data.Repositories
{
    public class ClubRepository(ClubServiceDbContext _context) : RepositoryBase(_context), IClubRepository
    {
        /// <inheritdoc />
        public async Task<ClubEntity> GetByIdAsync(Guid id, bool trackChanges = true)
        {
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new EntityNotFoundException($"Club with id '{id}' was not found."); ;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClubEntity>> GetAllAsync(bool trackChanges = true)
        {
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<PagedResponse<ClubEntity>> GetAllWithSpecificationAsync(
            int pageNumber = 1, 
            int pageSize = 500, 
            Dictionary<string, object>? filters = null, 
            KeyValuePair<string, SortDirection>? orderBy = null,
            bool trackChanges = true)
        {
            IQueryable<ClubEntity> query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<ClubEntity>.CreateFilters(filters));
            }
            if (orderBy != null)
            {
                var sortDirection = orderBy.Value.Value;
                var orderByExpression = QuerySpecificationFactory<ClubEntity>.CreateOrderBy(orderBy.Value.Key);

                query = sortDirection == SortDirection.Descending
                    ? query.OrderByDescending(orderByExpression)
                    : query.OrderBy(orderByExpression);
            }

            // Get count and Apply pagination
            int totalCount = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;
            var data = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedResponse<ClubEntity>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        /// <inheritdoc />
        public async Task<ClubEntity> CreateAsync(ClubEntity club)
        {
            ArgumentNullException.ThrowIfNull(club);

            // Detect duplicate - TODO: MOVE TO SERVICE LAYER
            var existingClub = await _context.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Email == club.Email);
            if (existingClub != null)
            {
                throw new DuplicateEntityException("A Club with the same email was found. Operation Canceled.");
            }
            // TODO - move to service
            club.SetTrackingFields();
            _context.Clubs.Add(club);
            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            return club;
        }

        /// <inheritdoc />
        public async Task<ClubEntity?> UpdateAsync(ClubEntity club)
        {
            ArgumentNullException.ThrowIfNull(club);
            // TODO - move to service
            club.SetTrackingFields(updating: true);
            _context.Entry(club).State = EntityState.Modified;

            if (!IsTransactionActive)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
                {
                    throw new EntityNotFoundException($"Club with id '{club.Id}' was not found.");
                }
            }

            return club;
        }

        /// <inheritdoc />
        public async Task<ClubEntity> PatchAsync(Guid id, IEnumerable<PropertyUpdate> propertyUpdates)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            var clubEntity = await GetByIdAsync(id) 
                ?? throw new EntityNotFoundException($"Club with id '{id}' was not found.");
            // Update
            clubEntity.Patch(propertyUpdates);
            // TODO - move to service
            clubEntity.SetTrackingFields(updating: true);

            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            return clubEntity;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid clubId)
        {
            var club = await _context.Clubs.FindAsync(clubId);
            if (club == null) return false;

            _context.Clubs.Remove(club);

            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
