using System.Net;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace MatchPoint.AccessControlService.Infrastructure.Data.Repositories
{
    public class CustomRoleRepository(AccessControlServiceDbContext _context, ILogger<CustomRoleRepository> _logger) :
        IRepository<CustomRoleEntity>
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
        public async Task<CustomRoleEntity?> CreateAsync(CustomRoleEntity customRoleEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(customRoleEntity);

            _logger.LogTrace("Creating a new custom role in the database with name {Name}", customRoleEntity.Name);

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
            _context.CustomRoles.Attach(customRoleEntity);
            _context.Entry(customRoleEntity).State = EntityState.Modified;

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
