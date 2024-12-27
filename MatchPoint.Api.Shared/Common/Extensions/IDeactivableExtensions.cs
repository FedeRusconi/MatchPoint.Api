using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.Api.Shared.Common.Extensions
{
    public static class IDeactivableExtensions
    {
        /// <summary>
        /// Returns true if ActiveStatus property equals <see cref="ActiveStatus.Active"/>
        /// </summary>
        /// <param name="deactivable"> The entity to check. </param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsActive(this IDeactivable deactivable)
        {
            return deactivable.ActiveStatus == ActiveStatus.Active;
        }
    }
}
