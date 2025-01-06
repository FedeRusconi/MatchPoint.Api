using Microsoft.Extensions.Configuration;

namespace MatchPoint.Api.Tests.Shared.Common.Helpers
{
    public class DataContextHelpers
    {
        public static IConfiguration TestingConfiguration => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Testing.json", optional: true) // Optional test-specific settings
            .AddEnvironmentVariables()
            .AddUserSecrets<DataContextHelpers>(optional: true) // Load user secrets
            .Build();
    }
}
