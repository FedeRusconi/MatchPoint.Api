using MatchPoint.Api.Shared.Common.Models;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IAzureAdUserFactory
    {
        /// <summary>
        /// Prepare a <see cref="User"/> instance with the information patched as per
        /// parameter of property updates.
        /// </summary>
        /// <param name="updates">
        /// A List of <see cref="PropertyUpdate"/> containing information about the updates to apply.
        /// </param>
        /// <param name="id"> The Id to assign to the user. Default is null. </param>
        /// <param name="extensionsClientId"> 
        /// The Guid of the system generated extension app registration in Azure. This is used for custom attributes. 
        /// </param>
        /// <returns><see cref="User"/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        User PatchedUser(IEnumerable<PropertyUpdate> updates, string? id = null, string? extensionsClientId = null);
    }
}