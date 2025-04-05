using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ServiceDefaults;

namespace MatchPoint.ClubService.Services
{
    public class CourtService(
        ICourtRepository _courtRepository,
        ISessionService _sessionService,
        ILogger<CourtService> _logger) : ICourtService
    {
        /// <inheritdoc />
        public async Task<IServiceResult<CourtEntity>> GetByIdAsync(Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve court with ID: {Id}", id);

            var courtEntity = await _courtRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            if (courtEntity == null)
            {
                _logger.LogWarning("Not Found: Court with Id: {Id} not found", id);
                return ServiceResult<CourtEntity>.Failure(
                    $"Court with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (courtEntity.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of court with ID: {Id} does not match provided '{clubId}'",
                    id,
                    clubId);
                return ServiceResult<CourtEntity>.Failure(
                    $"ClubId of court with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }

            _logger.LogDebug("Court with ID: {Id} found successfully", id);
            return ServiceResult<CourtEntity>.Success(courtEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<PagedResponse<CourtEntity>>> GetAllWithSpecificationAsync(
            Guid clubId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null)
        {
            // Validate params
            var paginationValidation = QuerySpecificationHelpers.ValidatePagination<CourtEntity>(pageNumber, pageSize);
            if (paginationValidation != null) return paginationValidation;

            var orderByValidation = QuerySpecificationHelpers.ValidateOrderBy<CourtEntity>(orderBy);
            if (orderByValidation != null) return orderByValidation;

            var filtersValidation = QuerySpecificationHelpers.ValidateFilters<CourtEntity>(filters);
            if (filtersValidation != null) return filtersValidation;

            _logger.LogDebug(
                "Attempting to retrieve courts with {Count} filters", filters != null ? filters.Count : "no");

            // Filter for club id is added automatically
            filters ??= [];
            filters.Add(nameof(CourtEntity.ClubId), clubId.ToString());
            var court = await _courtRepository.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, cancellationToken, filters, orderBy, trackChanges: false);

            _logger.LogDebug("Received {PageSize} of {Count} Courts found.", court.Data.Count(), court.TotalCount);

            return ServiceResult<PagedResponse<CourtEntity>>.Success(court);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<CourtEntity>> CreateAsync(
            Guid clubId, CourtEntity courtEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(courtEntity);

            _logger.LogDebug(
                "Attempting to create court with Name: {Name} for club '{ClubId}'", 
                courtEntity.Name, 
                courtEntity.ClubId);

            // Detect duplicate
            var filters = new Dictionary<string, string>()
            {
                { nameof(CourtEntity.Name), courtEntity.Name },
                { nameof(CourtEntity.ClubId), courtEntity.ClubId.ToString() }
            };
            var existingCourt = await _courtRepository.CountAsync(cancellationToken, filters);
            if (existingCourt > 0)
            {
                _logger.LogWarning("Conflict: Court with name '{Name}' already exists.", courtEntity.Name);
                return ServiceResult<CourtEntity>.Failure(
                    "A Court with the same name was found. Operation Canceled.", ServiceResultType.Conflict);
            }

            // Set ClubId and "Created" tracking fields
            courtEntity.SetTrackingFields(_sessionService.CurrentUserId);
            courtEntity.ClubId = clubId;

            // Create in db
            var createdEntity = await _courtRepository.CreateAsync(courtEntity, cancellationToken);
            if (createdEntity == null)
            {
                _logger.LogWarning("Conflict: Court with Id '{Id}' already exists.", courtEntity.Id);
                return ServiceResult<CourtEntity>.Failure(
                    $"Court with id '{courtEntity.Id}' already exists.", ServiceResultType.Conflict);
            }

            _logger.LogDebug(
                "Court with name '{Name}' for club '{ClubId}' created successfully. Id: {Id}", 
                createdEntity.Name, 
                createdEntity.ClubId,
                createdEntity.Id);
            return ServiceResult<CourtEntity>.Success(createdEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<CourtEntity>> PatchAsync(
            Guid clubId, Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            _logger.LogDebug("Attempting to patch court with Id: {Id}", id);

            // Find court
            var courtEntity = await _courtRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            if (courtEntity == null)
            {
                _logger.LogWarning("Not Found: Court with Id '{Id}' not found.", id);
                return ServiceResult<CourtEntity>.Failure(
                    $"Court with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (courtEntity.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of court with ID: {Id} does not match provided '{ClubId}'",
                    id,
                    clubId);
                return ServiceResult<CourtEntity>.Failure(
                    $"ClubId of court with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }

            try
            {
                courtEntity.Patch(propertyUpdates);
                courtEntity.SetTrackingFields(_sessionService.CurrentUserId, updating: true);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("{msg}", ex.Message);
                return ServiceResult<CourtEntity>.Failure(ex.Message, ServiceResultType.BadRequest);
            }

            var updatedEntity = await _courtRepository.UpdateAsync(courtEntity, cancellationToken);
            if (updatedEntity == null)
            {
                _logger.LogWarning("Not Found: Court with Id '{Id}' not found.", courtEntity.Id);
                return ServiceResult<CourtEntity>.Failure(
                    $"Court with id '{courtEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Court with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<CourtEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<CourtEntity>> DeleteAsync(Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to delete court with Id: {Id}", id);
            var court = await _courtRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            // Check court exists and its ClubId matches the provided one
            if (court == null)
            {
                _logger.LogWarning("Not Found: Court with Id '{Id}' not found.", id);
                return ServiceResult<CourtEntity>.Failure(
                    $"Court with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (court.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of court with ID: {Id} does not match provided '{ClubId}'",
                    id,
                    clubId);
                return ServiceResult<CourtEntity>.Failure(
                    $"ClubId of court with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }

            var deletedEntity = await _courtRepository.DeleteAsync(court, cancellationToken);
            if (deletedEntity == null)
            {
                _logger.LogWarning("Not Found: Court with Id '{Id}' not found.", id);
                return ServiceResult<CourtEntity>.Failure(
                    $"Court with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Court with Id '{Id}' deleted successfully.", deletedEntity.Id);
            return ServiceResult<CourtEntity>.Success(deletedEntity);
        }
    }
}
