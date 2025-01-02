using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MatchPoint.Api.Tests.Shared.Common.Helpers
{
    /// <summary>
    /// This class is used to mock the Authenticated user for integration tests
    /// </summary>
    public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        private const string ObjectIdClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public static readonly Guid ObjectIdValue = Guid.Parse("806d23a6-f382-4a81-a21b-6a761a74331d");
        public static readonly Guid ManagerObjectIdValue = Guid.Parse("3316286a-af59-430b-b506-fb341239288f");
        public static string Scopes { get; set; } = string.Empty;
        public static Claim[] Claims =>
            [
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim(ObjectIdClaim, ObjectIdValue.ToString()),
                new Claim("scp", Scopes)
            ];
        public static ClaimsIdentity Identity => new(Claims, "Test");
        public static ClaimsPrincipal Principal => new(Identity);

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var ticket = new AuthenticationTicket(Principal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
