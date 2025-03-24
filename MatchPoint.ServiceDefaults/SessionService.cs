using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

namespace MatchPoint.ServiceDefaults
{
    public interface ISessionService
    {
        /// <summary>
        /// The <see cref="Guid"/> of the user sending the request. 
        /// This is based on Token claims.
        /// </summary>
        Guid CurrentUserId { get; }
    }

    public class SessionService(IHttpContextAccessor _httpContextAccessor) : ISessionService
    {
        /// <inheritdoc />
        public Guid CurrentUserId => Guid.TryParse(_httpContextAccessor.HttpContext?.User.GetObjectId(), out var guid)
            ? guid
            : Guid.Empty;
    }
}
