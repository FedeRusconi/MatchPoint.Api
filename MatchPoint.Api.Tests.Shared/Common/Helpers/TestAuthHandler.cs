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
        public static readonly Guid ObjectIdValue = Guid.Parse("00122875-2bd4-4973-b7cb-bbbf3703d6d1");

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user"), new Claim(ObjectIdClaim, ObjectIdValue.ToString()) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
