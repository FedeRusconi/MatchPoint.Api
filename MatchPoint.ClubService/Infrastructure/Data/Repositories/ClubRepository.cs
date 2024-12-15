using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.Api.Shared.Infrastructure.RepositoryBases;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Infrastructure.Data.Repositories
{
    public class ClubRepository(ClubServiceDbContext _context, ILogger<ClubRepository> _logger) : 
        TransactionableRepositoryBase(_context), 
        IClubRepository
    {
        /// <inheritdoc />
        public async Task<int> CountAsync(Dictionary<string, string>? filters = null)
        {
            _logger.LogTrace(
                "Querying database for count of clubs with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<ClubEntity> query = _context.Clubs.AsNoTracking();

            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<ClubEntity>.CreateFilters(filters));
            }

            // Return count
            var count =  await query.CountAsync();
            _logger.LogTrace("Found {Count} Clubs in the database", count);
            return count;
        }

        /// <inheritdoc />
        public async Task<ClubEntity?> GetByIdAsync(Guid id, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for club with ID: '{Id}'", id);
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var club = await query.FirstOrDefaultAsync(c => c.Id == id);

            if (club == null)
            {
                _logger.LogWarning("No club found in the database with ID: {Id}", id);
            }
            else
            {
                _logger.LogTrace("Club with ID '{Id}' retrieved from the database", id);
            }

            return club;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClubEntity>> GetAllAsync(bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for all clubs");
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var clubs = await query.ToListAsync();
            _logger.LogTrace("Found {Count} Clubs in the database", clubs.Count);
            return clubs;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<ClubEntity>> GetAllWithSpecificationAsync(
            int pageNumber, 
            int pageSize, 
            Dictionary<string, string>? filters = null, 
            Dictionary<string, SortDirection>? orderBy = null,
            bool trackChanges = true)
        {
            _logger.LogTrace(
                "Querying database for clubs with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<ClubEntity> query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            if (filters != null)
            {
                query = query.WithFilters(filters);
            }
            if (orderBy != null)
            {
                query = query.WithOrderBy(orderBy);
            }

            // Get count and Apply pagination
            int totalCount = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;
            var data = await query.Skip(skip).Take(pageSize).ToListAsync();
            _logger.LogTrace("Returning {PageSize} of {Count} Clubs found in the database", pageSize, totalCount);
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

            _logger.LogTrace("Creating a new club in the database with email {Email}", clubEntity.Email);

            _context.Clubs.Add(clubEntity);
            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            _logger.LogTrace("New club '{Email}' added to the database", clubEntity.Email);
            return clubEntity;
        }

        /// <summary>
        /// Updates an existing <see cref="ClubEntity"/> in the database. 
        /// Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to update.</param>
        /// <returns> The updated <see cref="ClubEntity"/>. </returns>
        public async Task<ClubEntity?> UpdateAsync(ClubEntity clubEntity)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            _logger.LogTrace("Updating club in the database with Id {Id} ({Email})", clubEntity.Id, clubEntity.Email);

            _context.Entry(clubEntity).State = EntityState.Modified;

            if (!IsTransactionActive)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
                {
                    _logger.LogWarning("No club found in the database with ID: {Id}", clubEntity.Id);
                    return null;
                }
            }

            _logger.LogTrace("Club '{Id}' ({Email}) updated in the database", clubEntity.Id, clubEntity.Email);
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
        public async Task<ClubEntity?> DeleteAsync(Guid id)
        {
            var clubEntity = await _context.Clubs.FindAsync(id);
            if (clubEntity == null)
            {
                _logger.LogWarning("No club found in the database with ID: {Id}", id);
                return null;
            }

            _logger.LogTrace("Deleting club from the database with Id {Id} ({Email})", id, clubEntity.Email);

            _context.Clubs.Remove(clubEntity);

            if (!IsTransactionActive)
            {
                await _context.SaveChangesAsync();
            }

            _logger.LogTrace("Club '{Id}' ({Email}) deleted from the database", clubEntity.Id, clubEntity.Email);
            return clubEntity;
        }
    }
}
