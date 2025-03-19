using System.Net;
using Azure.Identity;
using MatchPoint.ClubService.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MatchPoint.ClubService.Services
{
    public class AzureAdService(IConfiguration _configuration) : IAzureAdService
    {
        private static readonly string[] UserParams =
            ["id", "mail", "givenName", "surname", "jobTitle", "mobilePhone", "BusinessPhones",
            "accountEnabled", "streetAddress", "city", "state", "postalCode", "country",
            "employeeHireDate", "employeeLeaveDateTime"];

        /// <inheritdoc />
        public GraphServiceClient GetGraphClient(string[]? scopes = null)
        {
            // Assign default values if not provided
            scopes ??= ["https://graph.microsoft.com/.default"];
            // Get AzureAd values
            string tenantId = _configuration.GetValue<string>("AzureAdB2C:TenantId")
                ?? throw new InvalidOperationException("Azure Tenant Id is null");
            string clientId = _configuration.GetValue<string>("AzureAdB2C:ClientId")
                ?? throw new InvalidOperationException("Azure Client Id is null");
            string clientSecret = _configuration.GetValue<string>("AzureAdB2C:ClientSecret")
                ?? throw new InvalidOperationException("Azure Client Secret is null");
            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            // Return client
            return new GraphServiceClient(clientSecretCredential, scopes);
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                return await client.Users[userId.ToString()].GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = UserParams;
                }, cancellationToken);
            }
            // If user is not found (404) simply return null
            catch (ODataError ex) when (ex.ResponseStatusCode == 404)
            {
                return null;
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<Stream?> GetUserPhotoAsync(Guid userId, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                return await client.Users[userId.ToString()].Photo.Content.GetAsync(cancellationToken: cancellationToken);
            }
            // If image is not found (404) simply return null
            catch (ODataError ex) when (ex.ResponseStatusCode == 404)
            {
                return null;
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<User?> GetUserManagerAsync(Guid userId, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                return await client.Users[userId.ToString()].Manager.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = UserParams;
                }, cancellationToken) as User;
            }
            // If manager is not found (404) simply return null
            catch (ODataError ex) when (ex.ResponseStatusCode == 404)
            {
                return null;
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                return await client.Users.PostAsync(user, cancellationToken: cancellationToken);
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<User?> UpdateUserAsync(User user, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                return await client.Users[user.Id].PatchAsync(user, cancellationToken: cancellationToken) ?? user;
            }
            // If user is not found (404) simply return null
            catch (ODataError ex) when (ex.ResponseStatusCode == 404)
            {
                return null;
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<Guid?> AssignUserManagerAsync(Guid userId, Guid managerId, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                var requestBody = new ReferenceUpdate()
                {
                    OdataId = $"https://graph.microsoft.com/v1.0/users/{managerId}",
                };
                await client.Users[userId.ToString()].Manager.Ref.PutAsync(requestBody, cancellationToken: cancellationToken);
                return managerId;
            }
            // If manager is not found (404) simply return null
            catch (ODataError ex) when (ex.ResponseStatusCode == 404)
            {
                return null;
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                await client.Users[userId.ToString()].DeleteAsync(cancellationToken: cancellationToken);
                return true;
            }
            // If user is not found (404) simply return null
            catch (ODataError ex) when (ex.ResponseStatusCode == 404)
            {
                return false;
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveUserManagerAsync(Guid userId, CancellationToken cancellationToken, GraphServiceClient? client = null)
        {
            // Use graph client provided or get a new one.
            client ??= GetGraphClient();
            try
            {
                await client.Users[userId.ToString()].Manager.Ref.DeleteAsync(cancellationToken: cancellationToken);
                return true;
            }
            // If user is not found (404) simply return null
            catch (ODataError ex) when (ex.ResponseStatusCode == 404)
            {
                return false;
            }
            catch (ODataError ex)
            {
                throw new HttpRequestException(ex.Message, ex, (HttpStatusCode)ex.ResponseStatusCode);
            }
        }
    }
}
