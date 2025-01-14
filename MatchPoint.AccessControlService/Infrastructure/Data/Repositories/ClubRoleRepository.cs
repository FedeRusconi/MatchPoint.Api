using System.Net;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Interfaces;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Extensions;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.AccessControlService.Infrastructure.Data.Repositories
{
    public class ClubRoleRepository(AccessControlServiceDbContext _context, ILogger<ClubRoleRepository> _logger) : 
        IClubRoleRepository
    {
        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            _logger.LogTrace(
                "Querying database for count of club roles with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<ClubRoleEntity> query = _context.ClubRoles.AsNoTracking();

            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<ClubRoleEntity>.CreateFilters(filters));
            }

            // Return count
            var count = await query.CountAsync(cancellationToken);
            _logger.LogTrace("Found {Count} club roles in the database", count);
            return count;
        }

        /// <inheritdoc />
        public async Task<ClubRoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for club role with ID: '{Id}'", id);
            var query = _context.ClubRoles.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var clubRole = await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (clubRole == null)
            {
                _logger.LogWarning("No club role found in the database with ID: {Id}", id);
            }
            else
            {
                _logger.LogTrace("Club role with ID '{Id}' retrieved from the database", id);
            }

            return clubRole;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClubRoleEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace(
                "Querying database for all club roles");
            IQueryable<ClubRoleEntity> query = _context.ClubRoles.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var clubRoles = await query.ToListAsync(cancellationToken);

            _logger.LogTrace("Returning {Count} club roles found in the database", clubRoles.Count);
            return clubRoles;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<ClubRoleEntity>> GetAllWithSpecificationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, Dictionary<string, string>? filters = null, Dictionary<string, SortDirection>? orderBy = null, bool trackChanges = true)
        {
            _logger.LogTrace(
                "Querying database for club roles with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<ClubRoleEntity> query = _context.ClubRoles.AsQueryable();

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
            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (pageNumber - 1) * pageSize;
            var data = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
            _logger.LogTrace("Returning {PageSize} of {Count} club roles found in the database", pageSize, totalCount);
            return new PagedResponse<ClubRoleEntity>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        /// <inheritdoc />
        public async Task<ClubRoleEntity?> CreateAsync(ClubRoleEntity clubRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubRoleEntity);

            _logger.LogTrace("Creating a new club role in the database with name {Name}", clubRoleEntity.Name);

            _context.ClubRoles.Add(clubRoleEntity);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Club role with specified id or name already exists.");
                return null;
            }

            _logger.LogTrace("New club role '{Name}' added to the database", clubRoleEntity.Name);
            return clubRoleEntity;
        }

        /// <inheritdoc />
        public async Task<ClubRoleEntity?> UpdateAsync(ClubRoleEntity clubRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubRoleEntity);

            _logger.LogTrace("Updating club role in the database with Id {Id} ({Name})", clubRoleEntity.Id, clubRoleEntity.Name);
            _context.ClubRoles.Attach(clubRoleEntity);
            _context.Entry(clubRoleEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
            {
                _logger.LogWarning("No club role found in the database with ID: {Id}", clubRoleEntity.Id);
                return null;
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No club role found in the database with ID: {Id}", clubRoleEntity.Id);
                return null;
            }

            _logger.LogTrace("Club role '{Id}' ({Name}) updated in the database", clubRoleEntity.Id, clubRoleEntity.Name);
            return clubRoleEntity;
        }

        /// <inheritdoc />
        public async Task<ClubRoleEntity?> DeleteAsync(ClubRoleEntity clubRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubRoleEntity);
            _logger.LogTrace("Deleting club role from the database with Id {Id} ({Name})", clubRoleEntity.Id, clubRoleEntity.Name);

            try
            {
                _context.ClubRoles.Remove(clubRoleEntity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No club role found in the database with ID: {Id}", clubRoleEntity.Id);
                return null;
            }

            _logger.LogTrace("Club role '{Id}' ({Name}) deleted from the database", clubRoleEntity.Id, clubRoleEntity.Name);
            return clubRoleEntity;
        }
    }
}
