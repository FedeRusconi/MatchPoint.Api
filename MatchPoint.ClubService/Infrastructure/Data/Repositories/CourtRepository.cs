using System.Net;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.ClubService.Infrastructure.Data.Repositories
{
    public class CourtRepository(ClubServiceDbContext _context, ILogger<CourtRepository> _logger) : ICourtRepository
    {
        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            _logger.LogTrace(
                "Querying database for count of courts with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<CourtEntity> query = _context.Courts.AsNoTracking();

            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<CourtEntity>.CreateFilters(filters));
            }

            // Return count
            var count = await query.CountAsync(cancellationToken);
            _logger.LogTrace("Found {Count} courts in the database", count);
            return count;
        }

        /// <inheritdoc />
        public async Task<CourtEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for court with ID: '{Id}'", id);
            var query = _context.Courts.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var court = await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (court == null)
            {
                _logger.LogWarning("No court found in the database with ID: {Id}", id);
            }
            else
            {
                _logger.LogTrace("Court with ID '{Id}' retrieved from the database", id);
            }

            return court;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CourtEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for all courts");
            var query = _context.Courts.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var courts = await query.ToListAsync(cancellationToken);
            _logger.LogTrace("Found {Count} courts in the database", courts.Count);
            return courts;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<CourtEntity>> GetAllWithSpecificationAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null,
            bool trackChanges = true)
        {
            _logger.LogTrace(
                "Querying database for courts with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<CourtEntity> query = _context.Courts.AsQueryable();

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
            _logger.LogTrace("Returning {PageSize} of {Count} courts found in the database", pageSize, totalCount);
            return new PagedResponse<CourtEntity>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        /// <inheritdoc />
        public async Task<CourtEntity?> CreateAsync(CourtEntity courtEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(courtEntity);

            _logger.LogTrace(
                "Creating a new court in the database with name {Name} for club '{ClubId}'", 
                courtEntity.Name,
                courtEntity.ClubId);

            courtEntity.ModifiedBy = null;
            courtEntity.ModifiedOnUtc = null;
            _context.Courts.Add(courtEntity);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Court with specified id already exists.");
                return null;
            }

            _logger.LogTrace(
                "New court '{Name}' for club '{clubId}' added to the database", 
                courtEntity.Name,
                courtEntity.ClubId);
            return courtEntity;
        }

        /// <inheritdoc />
        public async Task<CourtEntity?> UpdateAsync(CourtEntity courtEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(courtEntity);

            _logger.LogTrace("Updating court in the database with Id {Id}", courtEntity.Id);

            // Retrieve the existing entity first
            var existingEntity = await _context.Courts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courtEntity.Id, cancellationToken);
            if (existingEntity == null)
            {
                _logger.LogWarning("No court found in the database with Id: {Id}", courtEntity.Id);
                return null;
            }

            // Preserve properties that should not be updated
            courtEntity.CreatedBy = existingEntity.CreatedBy;
            courtEntity.CreatedOnUtc = existingEntity.CreatedOnUtc;
            _context.Courts.Update(courtEntity);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
            {
                _logger.LogWarning("No court found in the database with Id: {Id}", courtEntity.Id);
                return null;
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No court found in the database with Id: {Id}", courtEntity.Id);
                return null;
            }

            _logger.LogTrace("Court '{Id}' updated in the database", courtEntity.Id);
            return courtEntity;
        }

        /// <inheritdoc />
        public async Task<CourtEntity?> DeleteAsync(CourtEntity courtEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(courtEntity);
            _logger.LogTrace("Deleting court from the database with Id {Id}", courtEntity.Id);

            try
            {
                _context.Courts.Remove(courtEntity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No court found in the database with Id: {Id}", courtEntity.Id);
                return null;
            }

            _logger.LogTrace("Court '{Id}' deleted from the database", courtEntity.Id);
            return courtEntity;
        }
    }
}
