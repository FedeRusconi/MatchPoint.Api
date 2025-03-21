using System.Net;
using System.Text;
using System.Text.Json;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.Api.Tests.Shared.AccessControlService.Helpers
{
    /// <summary>
    /// This class is used to mock the ClubRole of the current user for integration tests of the controllers.
    /// This is neeeded to avoid having to create an actual ClubRole and a User just to pass the RBAC check.
    /// </summary>
    public class MockClubRoleHandler(Guid _clubId, Guid _roleId) : DelegatingHandler
    {
        private readonly string _targetUrl = $"api/v1/clubs/{_clubId}/roles/{_roleId}";
        private RoleCapabilityFeature _feature;
        private RoleCapabilityAction _action;

        public MockClubRoleHandler ForFeature(RoleCapabilityFeature feature)
        {
            _feature = feature;
            return this;
        }

        public MockClubRoleHandler WithAction(RoleCapabilityAction action)
        {
            _action = action;
            return this;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Check if the request matches the URL, if so return a mock ClubRole
            if (request.RequestUri != null && request.RequestUri.ToString().EndsWith(_targetUrl))
            {
                // Mocked ClubRole object
                ClubRole mockClubRole = new()
                {
                    Id = _roleId,
                    Name = "Mock Club Role",
                    ClubId = _clubId,
                    Capabilities = [
                        new(){ Id = Guid.NewGuid(), Feature = _feature, Action = _action }
                    ]
                };

                // Set Content for response
                var jsonResponse = JsonSerializer.Serialize(mockClubRole);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                };

                return await Task.FromResult(response);
            }

            // Pass through to the real handler if no match
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
