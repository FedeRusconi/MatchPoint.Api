using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;
using MatchPoint.Api.Tests.Shared.Common.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MatchPoint.Api.Tests.Shared.Common.Extensions
{
    public static class WebApplicationFactoryExtensions
    {
        /// <summary>
        /// Prepares and returns an <see cref="HttpClient"/>
        /// specifically set for MatchPoint integration tests for controllers.
        /// This Defines a test AuthHandler and test IConfiguration.
        /// </summary>
        /// <typeparam name="T"> The Program class. </typeparam>
        /// <param name="factory"> 
        /// The <see cref="WebApplicationFactory{TEntryPoint}"/> used as the source
        /// </param>
        /// <param name="authenticated"> 
        /// Whether the HttpClient should mock an authenticated user or not. Default is true.
        /// </param>
        /// <returns> An <see cref="HttpClient"/>. </returns>
        public static HttpClient GetTestHttpClient<T>(
            this WebApplicationFactory<T> factory,
            bool authenticated = true) where T : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Set test authentication
                    if (authenticated)
                    {
                        services.AddAuthentication(defaultScheme: "TestScheme")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "TestScheme", options => { });
                    }
                    else
                    {
                        services.AddAuthentication();
                    }

                    // Replace IConfiguration in the DI container with test-specific configuration
                    services.RemoveAll<IConfiguration>();
                    services.AddSingleton(DataContextHelpers.TestingConfiguration);
                });
            })
            .CreateClient();
        }

        /// <summary>
        /// Prepares and returns an <see cref="HttpClient"/>
        /// specifically set for MatchPoint integration tests for controllers.
        /// This Defines a test AuthHandler, test IConfiguration and a mock Handler for
        /// AccessControlService http client, to mock the call to retrieve the user's ClubRole.
        /// </summary>
        /// <typeparam name="T"> The Program class. </typeparam>
        /// <param name="factory"> 
        /// The <see cref="WebApplicationFactory{TEntryPoint}"/> used as the source
        /// </param>
        /// <param name="clubId"> The defined Club id. </param>
        /// <param name="roleId"> The defined ClubRole id. </param>
        /// <param name="feature"> 
        /// A <see cref="RoleCapabilityFeature"/> to set within the mock ClubRole. 
        /// </param>
        /// <param name="action"> 
        /// A <see cref="RoleCapabilityAction"/> to set within the mock ClubRole for the given feature. 
        /// </param>
        /// <param name="authenticated"> 
        /// Whether the HttpClient should mock an authenticated user or not. Default is true.
        /// </param>
        /// <returns> An <see cref="HttpClient"/>. </returns>
        public static HttpClient GetTestHttpClientWithRoleCheck<T>(
            this WebApplicationFactory<T> factory,
            Guid clubId,
            Guid roleId,
            RoleCapabilityFeature feature,
            RoleCapabilityAction action,
            bool authenticated = true) where T : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Set test authentication
                    if (authenticated)
                    {
                        services.AddAuthentication(defaultScheme: "TestScheme")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "TestScheme", options => { });
                    }
                    else
                    {
                        services.AddAuthentication();
                    }

                    // Replace IConfiguration in the DI container with test-specific configuration
                    services.RemoveAll<IConfiguration>();
                    services.AddSingleton(DataContextHelpers.TestingConfiguration);

                    // Override the HttpClient for the AccessControlService
                    var mockHandler = new MockClubRoleHandler(clubId, roleId)
                                .ForFeature(feature)
                                .WithAction(action);
                    services.AddHttpClient("AccessControlService")
                        .ConfigurePrimaryHttpMessageHandler(() => mockHandler);
                });
            })
            .CreateClient();
        }
    }
}
