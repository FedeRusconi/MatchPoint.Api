using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.Api.Shared.Common.Utilities;
using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;
using MatchPoint.Api.Shared.Infrastructure.Utilities;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using MatchPoint.ClubService.Mappers;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Services
{
    public class ClubStaffService(
        IClubStaffRepository _clubStaffRepository,
        IAzureAdService _azureAdService,
        IAzureAdUserFactory _azureAdUserFactory,
        IConfiguration _configuration,        
        ILogger<ClubStaffService> _logger) : IClubStaffService
    {
        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> GetByIdAsync(Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve club staff with ID: {Id}", id);

            var clubStaffEntity = await _clubStaffRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            if (clubStaffEntity == null)
            {
                _logger.LogWarning("Not Found: Club staff with ID: {Id} not found", id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (clubStaffEntity.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of club staff with ID: {Id} does not match provided '{clubId}'", 
                    id,
                    clubId);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"ClubID of club staff with id '{id}' does not match provided '{clubId}'.", 
                    ServiceResultType.BadRequest);
            }

            _logger.LogDebug("Club staff with ID: {Id} found successfully", id);
            return ServiceResult<ClubStaffEntity>.Success(clubStaffEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<PagedResponse<ClubStaffEntity>>> GetAllWithSpecificationAsync(
            Guid clubId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken,
            Dictionary<string, string>? filters = null,
            Dictionary<string, SortDirection>? orderBy = null)
        {
            // Validate params
            var paginationValidation = QuerySpecificationHelpers.ValidatePagination<ClubStaffEntity>(pageNumber, pageSize);
            if (paginationValidation != null) return paginationValidation;

            var orderByValidation = QuerySpecificationHelpers.ValidateOrderBy<ClubStaffEntity>(orderBy);
            if (orderByValidation != null) return orderByValidation;

            var filtersValidation = QuerySpecificationHelpers.ValidateFilters<ClubStaffEntity>(filters);
            if (filtersValidation != null) return filtersValidation;

            _logger.LogDebug(
                "Attempting to retrieve club staff with {Count} filters", filters != null ? filters.Count : "no");

            // Filter for club id is added automatically
            filters ??= [];
            filters.Add(nameof(ClubStaffEntity.ClubId), clubId.ToString());
            var clubStaff = await _clubStaffRepository.GetAllWithSpecificationAsync(
                    pageNumber, pageSize, cancellationToken, filters, orderBy, trackChanges: false);

            _logger.LogDebug("Receieved {PageSize} of {Count} Club staff found.", clubStaff.Data.Count(), clubStaff.TotalCount);

            return ServiceResult<PagedResponse<ClubStaffEntity>>.Success(clubStaff);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> CreateAsync(
            Guid clubId, ClubStaffEntity clubStaffEntity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(clubStaffEntity);

            _logger.LogDebug("Attempting to create club staff with Email: {Email}", clubStaffEntity.Email);

            // Detect duplicate
            var filters = new Dictionary<string, string>()
            {
                { nameof(ClubStaffEntity.Email), clubStaffEntity.Email }
            };
            var existingStaff = await _clubStaffRepository.CountAsync(cancellationToken, filters);
            if (existingStaff > 0)
            {
                _logger.LogWarning("Conflict: Club staff with email '{Email}' already exists.", clubStaffEntity.Email);
                return ServiceResult<ClubStaffEntity>.Failure(
                    "A Club staff with the same email was found. Operation Canceled.", ServiceResultType.Conflict);
            }

            var azureDomain = _configuration.GetValue<string>("AzureAdB2C:Domain");
            // First create Azure AD user
            var azureAdUser = clubStaffEntity.ToAzureAdUser();
            azureAdUser.DisplayName = $"{azureAdUser.GivenName} {azureAdUser.Surname}";
            azureAdUser.MailNickname = $"{azureAdUser.GivenName?.Replace(" ", string.Empty)}.{azureAdUser.Surname?.Replace(" ", string.Empty)}";
            azureAdUser.UserPrincipalName = $"{clubStaffEntity.Email.Replace("@", "_")}@{azureDomain}";
            azureAdUser.Identities = [new ObjectIdentity()
            {
                SignInType = "emailAddress",
                Issuer = azureDomain,
                IssuerAssignedId = clubStaffEntity.Email
            }];
            azureAdUser.PasswordProfile = new()
            {
                Password = PasswordGenerator.GenerateNumeric(6, prefix: "MatchPoint_")
            };
            // TODO - Send email to staff email with temp password
            try
            {
                var azureResult = await _azureAdService.CreateUserAsync(azureAdUser, cancellationToken);
                azureAdUser.Id = azureResult?.Id;
                _logger.LogDebug(
                    "User with email '{Email}' created successfully in Azure AD. Id: {Id}", clubStaffEntity.Email, azureResult?.Id);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning("Error while creating user '{Email}' in Azure AD. Error: {Error}", clubStaffEntity.Email, ex.Message);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Error while crearing user '{clubStaffEntity.Email}' in Azure AD. Error: {ex.Message}", (ServiceResultType)(int)ex.StatusCode!);
            }

            // Set Id and "Created" tracking fields
            clubStaffEntity.Id = Guid.Parse(azureAdUser.Id!);
            clubStaffEntity.SetTrackingFields(_azureAdService.CurrentUserId);
            clubStaffEntity.ClubId = clubId;

            // Create in db
            var createdEntity = await _clubStaffRepository.CreateAsync(clubStaffEntity, cancellationToken);
            if (createdEntity == null)
            {
                // Rollback - Delete user from Azure AD
                await _azureAdService.DeleteUserAsync(clubStaffEntity.Id, cancellationToken);
                // Log
                _logger.LogWarning("Conflict: Club staff with Id '{Id}' already exists.", clubStaffEntity.Id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{clubStaffEntity.Id}' already exists.", ServiceResultType.Conflict);
            }

            _logger.LogDebug(
                "Club staff with email '{Email}' created successfully. Id: {Id}", createdEntity.Email, createdEntity.Id);
            return ServiceResult<ClubStaffEntity>.Success(createdEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> PatchAsync(
            Guid clubId, Guid id, IEnumerable<PropertyUpdate> propertyUpdates, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(propertyUpdates);

            _logger.LogDebug("Attempting to patch club staff with Id: {Id}", id);

            // Find club
            var clubStaffEntity = await _clubStaffRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);            
            // Keep this as a backup in case of rollback
            var azureAdUser = await _azureAdService.GetUserByIdAsync(id, cancellationToken);
            if (clubStaffEntity == null || azureAdUser == null)
            {
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (clubStaffEntity.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of club staff with ID: {Id} does not match provided '{ClubId}'",
                    id,
                    clubId);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"ClubID of club staff with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }

            try
            {
                clubStaffEntity.Patch(propertyUpdates);
                clubStaffEntity.SetTrackingFields(_azureAdService.CurrentUserId, updating: true);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("{msg}", ex.Message);
                return ServiceResult<ClubStaffEntity>.Failure(ex.Message, ServiceResultType.BadRequest);
            }

            try
            {
                // Update user is Azure AD
                var updatedAdUser = _azureAdUserFactory.PatchedUser(propertyUpdates, id.ToString());
                // Reset display name and mail nickname in case first or last name have changed
                var givenName = updatedAdUser.GivenName ?? azureAdUser.GivenName;
                var surname = updatedAdUser.Surname ?? azureAdUser.Surname;
                updatedAdUser.DisplayName = $"{givenName} {surname}";
                updatedAdUser.MailNickname = $"{givenName?.Replace(" ", string.Empty)}.{surname?.Replace(" ", string.Empty)}";
                await _azureAdService.UpdateUserAsync(updatedAdUser, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning("Error while updating user '{Id}' from Azure AD. Error: {Error}", id, ex.Message);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Error while updating user '{id}' from Azure AD. Error: {ex.Message}", (ServiceResultType)(int)ex.StatusCode!);
            }
            // Update club staff in DB
            var updatedEntity = await _clubStaffRepository.UpdateAsync(clubStaffEntity, cancellationToken);
            if (updatedEntity == null)
            {
                // Rollback in Azure AD
                await _azureAdService.UpdateUserAsync(azureAdUser, cancellationToken: cancellationToken);
                // Log
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", clubStaffEntity.Id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{clubStaffEntity.Id}' was not found.", ServiceResultType.NotFound);
            }

            _logger.LogDebug(
                "Club staff with Id '{Id}' updated successfully.", updatedEntity.Id);
            return ServiceResult<ClubStaffEntity>.Success(updatedEntity);
        }

        /// <inheritdoc />
        public async Task<IServiceResult<ClubStaffEntity>> DeleteAsync(Guid clubId, Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to delete club staff with Id: {Id}", id);
            var clubStaff = await _clubStaffRepository.GetByIdAsync(id, cancellationToken, trackChanges: false);
            // Check staff exists and its ClubId matches the provided one
            if (clubStaff == null)
            {
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{id}' was not found.", ServiceResultType.NotFound);
            }
            if (clubStaff.ClubId != clubId)
            {
                _logger.LogWarning(
                    "Bad Request: ClubId of club staff with ID: {Id} does not match provided '{ClubId}'",
                    id,
                    clubId);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"ClubID of club staff with id '{id}' does not match provided '{clubId}'.",
                    ServiceResultType.BadRequest);
            }

            var deletedEntity = await _clubStaffRepository.DeleteAsync(clubStaff, cancellationToken);
            if (deletedEntity == null)
            {
                _logger.LogWarning("Not Found: Club staff with Id '{Id}' not found.", id);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Club staff with id '{id}' was not found.", ServiceResultType.NotFound);
            }

            try
            {
                await _azureAdService.DeleteUserAsync(id, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                // Rollback - Re-created deleted club staff from db
                await _clubStaffRepository.CreateAsync(deletedEntity, cancellationToken);
                _logger.LogWarning("Error while deleting user '{Id}' from Azure AD. Error: {Error}", id, ex.Message);
                return ServiceResult<ClubStaffEntity>.Failure(
                    $"Error while deleting user '{id}' from Azure AD. Error: {ex.Message}", (ServiceResultType)(int)ex.StatusCode!);
            }

            _logger.LogDebug(
                "Club staff with Id '{Id}' deleted successfully.", deletedEntity.Id);
            return ServiceResult<ClubStaffEntity>.Success(deletedEntity);
        }
    }
}
