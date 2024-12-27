using MatchPoint.Api.Shared.Common.Models;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IAzureAdUserFactory
    {
        User PatchedUser(IEnumerable<PropertyUpdate> updates, string? id = null);
    }
}