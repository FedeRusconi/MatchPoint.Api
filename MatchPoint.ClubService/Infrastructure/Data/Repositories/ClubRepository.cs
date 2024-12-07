using MatchPoint.Api.Shared.Enums;
using MatchPoint.Api.Shared.Exceptions;
using MatchPoint.Api.Shared.Models;
using MatchPoint.Api.Shared.Repositories;
using MatchPoint.Api.Shared.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Infrastructure.Data.Repositories
{
    public class ClubRepository(ClubServiceDbContext _context) : 
        TransactionableRepositoryBase(_context), 
        IClubRepository
    {
        /// <inheritdoc />
        public async Task<int> CountAsync(Dictionary<string, object>? filters = null)
        {
            IQueryable<ClubEntity> query = _context.Clubs.AsNoTracking();

            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<ClubEntity>.CreateFilters(filters));
            }

            // Return count
            return await query.CountAsync();
        }

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
            int pageNumber, 
            int pageSize, 
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

        /// <summary>
        /// Adds a new <see cref="ClubEntity"/> to the database. 
        /// Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to add.</param>
        /// <returns> The newly created <see cref="ClubEntity"/>. </returns>
        public async Task<ClubEntity> CreateAsync(ClubEntity clubEntity)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);
            
            _context.Clubs.Add(clubEntity);
            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            return clubEntity;
        }

        /// <summary>
        /// Updates an existing <see cref="ClubEntity"/> in the database. 
        /// Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to update.</param>
        /// <returns> The updated <see cref="ClubEntity"/>. </returns>
        public async Task<ClubEntity> UpdateAsync(ClubEntity clubEntity)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            _context.Entry(clubEntity).State = EntityState.Modified;

            if (!IsTransactionActive)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
                {
                    throw new EntityNotFoundException($"Club with id '{clubEntity.Id}' was not found.");
                }
            }

            return clubEntity;
        }

        /// <summary>
        /// Deletes a <see cref="ClubEntity"/> by its unique identifier.
        /// Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>
        /// True if successful, false if no <see cref="ClubEntity"/> with matching Id was found.
        /// </returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var club = await _context.Clubs.FindAsync(id);
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
