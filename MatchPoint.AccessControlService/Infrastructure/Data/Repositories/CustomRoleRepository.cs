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
    public class CustomRoleRepository(AccessControlServiceDbContext _context, ILogger<CustomRoleRepository> _logger) :
        ICustomRoleRepository
    {
        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken, Dictionary<string, string>? filters = null)
        {
            _logger.LogTrace(
                "Querying database for count of custom roles with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<CustomRoleEntity> query = _context.CustomRoles.AsNoTracking();

            if (filters != null)
            {
                query = query.Where(QuerySpecificationFactory<CustomRoleEntity>.CreateFilters(filters));
            }

            // Return count
            var count = await query.CountAsync(cancellationToken);
            _logger.LogTrace("Found {Count} custom roles in the database", count);
            return count;
        }

        /// <inheritdoc />
        public async Task<CustomRoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace("Querying database for custom role with ID: '{Id}'", id);
            var query = _context.CustomRoles.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var customRole = await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (customRole == null)
            {
                _logger.LogWarning("No custom role found in the database with ID: {Id}", id);
            }
            else
            {
                _logger.LogTrace("Custom role with ID '{Id}' retrieved from the database", id);
            }

            return customRole;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CustomRoleEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = true)
        {
            _logger.LogTrace(
                "Querying database for all custom roles");
            IQueryable<CustomRoleEntity> query = _context.CustomRoles.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var customRoles = await query.ToListAsync(cancellationToken);

            _logger.LogTrace("Returning {Count} custom roles found in the database", customRoles.Count);
            return customRoles;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<CustomRoleEntity>> GetAllWithSpecificationAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken, 
            Dictionary<string, string>? filters = null, 
            Dictionary<string, SortDirection>? orderBy = null, 
            bool trackChanges = true)
        {
            _logger.LogTrace(
                "Querying database for custom roles with {Count} filters", filters != null ? filters.Count : "no");
            IQueryable<CustomRoleEntity> query = _context.CustomRoles.AsQueryable();

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
            _logger.LogTrace("Returning {PageSize} of {Count} custom roles found in the database", pageSize, totalCount);
            return new PagedResponse<CustomRoleEntity>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        /// <inheritdoc />
        public async Task<CustomRoleEntity?> CreateAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(customRoleEntity);

            _logger.LogTrace("Creating a new custom role in the database with name {Name}", customRoleEntity.Name);

            customRoleEntity.ModifiedBy = null;
            customRoleEntity.ModifiedOnUtc = null;
            _context.CustomRoles.Add(customRoleEntity);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Custom role with specified id or name already exists.");
                return null;
            }

            _logger.LogTrace("New custom role '{Name}' added to the database", customRoleEntity.Name);
            return customRoleEntity;
        }

        /// <inheritdoc />
        public async Task<CustomRoleEntity?> UpdateAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(customRoleEntity);

            _logger.LogTrace("Updating custom role in the database with Id {Id} ({Name})", customRoleEntity.Id, customRoleEntity.Name);

            // Retrieve the existing entity first
            var existingEntity = await _context.CustomRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customRoleEntity.Id, cancellationToken);
            if (existingEntity == null)
            {
                _logger.LogWarning("No custom role found in the database with ID: {Id}", customRoleEntity.Id);
                return null;
            }

            // Preserve properties that should not be updated
            customRoleEntity.CreatedBy = existingEntity.CreatedBy;
            customRoleEntity.CreatedOnUtc = existingEntity.CreatedOnUtc;
            _context.CustomRoles.Update(customRoleEntity);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is NullReferenceException)
            {
                _logger.LogWarning("No custom role found in the database with ID: {Id}", customRoleEntity.Id);
                return null;
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No custom role found in the database with ID: {Id}", customRoleEntity.Id);
                return null;
            }

            _logger.LogTrace("Custom role '{Id}' ({Name}) updated in the database", customRoleEntity.Id, customRoleEntity.Name);
            return customRoleEntity;
        }

        /// <inheritdoc />
        public async Task<CustomRoleEntity?> DeleteAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(customRoleEntity);
            _logger.LogTrace("Deleting custom role from the database with Id {Id} ({Name})", customRoleEntity.Id, customRoleEntity.Name);

            try
            {
                _context.CustomRoles.Remove(customRoleEntity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is CosmosException cosmosEx && cosmosEx.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No custom role found in the database with ID: {Id}", customRoleEntity.Id);
                return null;
            }

            _logger.LogTrace("Custom role '{Id}' ({Name}) deleted from the database", customRoleEntity.Id, customRoleEntity.Name);
            return customRoleEntity;
        }
    }
}
