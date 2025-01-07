using System.Net;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.Api.Shared.Infrastructure.RepositoryBases;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Infrastructure.Data.Repositories
{
    public class ClubRepository(ClubServiceDbContext _context, ILogger<ClubRepository> _logger) : 
        TransactionableRepositoryBase(_context), 
        IClubRepository
    {
        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            _logger.LogTrace(
                "Querying database for count of clubs with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<ClubEntity> query = _context.Clubs.AsNoTracking();

            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<ClubEntity>.CreateFilters(filters));
            }

            // Return count
            var count =  await query.CountAsync(cancellationToken);
            _logger.LogTrace("Found {Count} clubs in the database", count);
            return count;
        }

        /// <inheritdoc />
        public async Task<ClubEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for club with ID: '{Id}'", id);
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var club = await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

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
        public async Task<IEnumerable<ClubEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for all clubs");
            var query = _context.Clubs.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var clubs = await query.ToListAsync(cancellationToken);
            _logger.LogTrace("Found {Count} clubs in the database", clubs.Count);
            return clubs;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<ClubEntity>> GetAllWithSpecificationAsync(
            int pageNumber, 
            int pageSize,
            CancellationToken cancellationToken,
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
            var data = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
            _logger.LogTrace("Returning {PageSize} of {Count} clubs found in the database", pageSize, totalCount);
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
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <returns> The newly created <see cref="ClubEntity"/>. </returns>
        public async Task<ClubEntity?> CreateAsync(ClubEntity clubEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            _logger.LogTrace("Creating a new club in the database with email {Email}", clubEntity.Email);

            _context.Clubs.Add(clubEntity);
            if (!IsTransactionActive)
            {
                try
                {
                    await CommitTransactionAsync(cancellationToken);
                }
                catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.Conflict)
                {
                    _logger.LogWarning("Club with specified id or name already exists.");
                    return null;
                }
            }

            _logger.LogTrace("New club '{Email}' added to the database", clubEntity.Email);
            return clubEntity;
        }

        /// <summary>
        /// Updates an existing <see cref="ClubEntity"/> in the database. 
        /// Saves the changes immediately if no active transaction exists.
        /// </summary>
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to update.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <returns> The updated <see cref="ClubEntity"/>. </returns>
        public async Task<ClubEntity?> UpdateAsync(ClubEntity clubEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);

            _logger.LogTrace("Updating club in the database with Id {Id} ({Email})", clubEntity.Id, clubEntity.Email);
            _context.Clubs.Attach(clubEntity);
            _context.Entry(clubEntity).State = EntityState.Modified;

            if (!IsTransactionActive)
            {
                try
                {
                    await CommitTransactionAsync(cancellationToken);
                }
                catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
                {
                    _logger.LogWarning("No club found in the database with ID: {Id}", clubEntity.Id);
                    return null;
                }
                catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
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
        /// <param name="clubEntity">The <see cref="ClubEntity"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests, freeing up resources if the request is canceled.
        /// </param>
        /// <returns>
        /// True if successful, false if no <see cref="ClubEntity"/> with matching Id was found.
        /// </returns>
        public async Task<ClubEntity?> DeleteAsync(ClubEntity clubEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubEntity);
            _logger.LogTrace("Deleting club from the database with Id {Id} ({Email})", clubEntity.Id, clubEntity.Email);

            try
            {
                _context.Clubs.Remove(clubEntity);
                if (!IsTransactionActive)
                {
                    await CommitTransactionAsync(cancellationToken);
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No club found in the database with ID: {Id}", clubEntity.Id);
                return null;
            }

            _logger.LogTrace("Club '{Id}' ({Email}) deleted from the database", clubEntity.Id, clubEntity.Email);
            return clubEntity;
        }
    }
}
