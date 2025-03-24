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
    public class ClubStaffRepository(ClubServiceDbContext _context, ILogger<ClubStaffRepository> _logger) : 
        IClubStaffRepository
    {
        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            _logger.LogTrace(
                "Querying database for count of club staff with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<ClubStaffEntity> query = _context.ClubStaff.AsNoTracking();

            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<ClubStaffEntity>.CreateFilters(filters));
            }

            // Return count
            var count = await query.CountAsync(cancellationToken);
            _logger.LogTrace("Found {Count} club staff in the database", count);
            return count;
        }

        /// <inheritdoc />
        public async Task<ClubStaffEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for club staff with ID: '{Id}'", id);
            var query = _context.ClubStaff.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var staff = await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (staff == null)
            {
                _logger.LogWarning("No club staff found in the database with ID: {Id}", id);
            }
            else
            {
                _logger.LogTrace("Club staff with ID '{Id}' retrieved from the database", id);
            }

            return staff;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClubStaffEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for all club staff");
            var query = _context.ClubStaff.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var staff = await query.ToListAsync(cancellationToken);
            _logger.LogTrace("Found {Count} club staff in the database", staff.Count);
            return staff;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<ClubStaffEntity>> GetAllWithSpecificationAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null,
            bool trackChanges = true)
        {
            _logger.LogTrace(
                "Querying database for club staff with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<ClubStaffEntity> query = _context.ClubStaff.AsQueryable();

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
            _logger.LogTrace("Returning {PageSize} of {Count} club staff found in the database", pageSize, totalCount);
            return new PagedResponse<ClubStaffEntity>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        /// <inheritdoc />
        public async Task<ClubStaffEntity?> CreateAsync(ClubStaffEntity clubStaffEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubStaffEntity);

            _logger.LogTrace("Creating a new club staff in the database with id {Id}", clubStaffEntity.Id);

            clubStaffEntity.ModifiedBy = null;
            clubStaffEntity.ModifiedOnUtc = null;
            _context.ClubStaff.Add(clubStaffEntity);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Club staff with specified id already exists.");
                return null;
            }

            _logger.LogTrace("New club staff with id '{Id}' added to the database", clubStaffEntity.Id);
            return clubStaffEntity;
        }

        /// <inheritdoc />
        public async Task<ClubStaffEntity?> UpdateAsync(ClubStaffEntity clubStaffEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubStaffEntity);

            _logger.LogTrace("Updating club staff in the database with Id {Id}", clubStaffEntity.Id);

            // Retrieve the existing entity first
            var existingEntity = await _context.ClubStaff
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clubStaffEntity.Id, cancellationToken);
            if (existingEntity == null)
            {
                _logger.LogWarning("No club staff found in the database with ID: {Id}", clubStaffEntity.Id);
                return null;
            }

            // Preserve properties that should not be updated
            clubStaffEntity.CreatedBy = existingEntity.CreatedBy;
            clubStaffEntity.CreatedOnUtc = existingEntity.CreatedOnUtc;
            _context.ClubStaff.Update(clubStaffEntity);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
            {
                _logger.LogWarning("No club staff found in the database with ID: {Id}", clubStaffEntity.Id);
                return null;
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No club staff found in the database with ID: {Id}", clubStaffEntity.Id);
                return null;
            }

            _logger.LogTrace("Club staff '{Id}' updated in the database", clubStaffEntity.Id);
            return clubStaffEntity;
        }

        /// <inheritdoc />
        public async Task<ClubStaffEntity?> DeleteAsync(ClubStaffEntity clubStaffEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubStaffEntity);
            _logger.LogTrace("Deleting club staff from the database with Id {Id}", clubStaffEntity.Id);

            try
            {
                _context.ClubStaff.Remove(clubStaffEntity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No club staff found in the database with ID: {Id}", clubStaffEntity.Id);
                return null;
            }

            _logger.LogTrace("Club staff '{Id}' deleted from the database", clubStaffEntity.Id);
            return clubStaffEntity;
        }
    }
}
