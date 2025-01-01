using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Extensions;
using MatchPoint.Api.Shared.Common.Interfaces;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Interfaces;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Infrastructure.Data.Factories
{
    /// <summary>
    /// This class is simply a wrapper for <see cref="User"/>
    /// to allow the <see cref="IPatchable"/> interface and the Patch method from extensions.
    /// </summary>
    public class PatchableAzureUser : User, IPatchable
    { }

    public class AzureAdUserFactory : IAzureAdUserFactory
    {
        /// <summary>
        /// Mapping between <see cref="ClubStaffEntity"/> and <see cref="User"/> properties.
        /// </summary>
        private static readonly Dictionary<string, string> _propertiesMapping = new()
        {
            { nameof(ClubStaffEntity.FirstName), nameof(User.GivenName) },
            { nameof(ClubStaffEntity.LastName), nameof(User.Surname) },
            { nameof(ClubStaffEntity.JobTitle), nameof(User.JobTitle) },
            { nameof(ClubStaffEntity.PhoneNumber), nameof(User.MobilePhone) },
            { nameof(ClubStaffEntity.HiredOnUtc), nameof(User.EmployeeHireDate) },
            { nameof(ClubStaffEntity.LeftOnUtc), nameof(User.EmployeeLeaveDateTime) }
        };

        /// <summary>
        /// Prepare a <see cref="User"/> instance with the information patched as per
        /// parameter of property updates.
        /// </summary>
        /// <param name="updates">
        /// A List of <see cref="PropertyUpdate"/> containing information about the updates to apply.
        /// </param>
        /// <param name="id"> The Id to assign to the user. Default is null. </param>
        /// <returns><see cref="User"/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public User PatchedUser(IEnumerable<PropertyUpdate> updates, string? id = null)
        {
            ArgumentNullException.ThrowIfNull(updates, nameof(updates));

            var adUser = new PatchableAzureUser()
            {
                Id = id
            };
            List<PropertyUpdate> userUpdates = [];
            foreach (var patchProperty in updates)
            {
                // Handle the special cases first
                if (patchProperty.Property == nameof(ClubStaffEntity.Address))
                {
                    var addressValue = patchProperty.Value as Address;
                    adUser.StreetAddress = addressValue?.Street;
                    adUser.City = addressValue?.City;
                    adUser.State = addressValue?.State;
                    adUser.PostalCode = addressValue?.PostalCode;
                    adUser.Country = addressValue?.Country.Name;
                    continue;
                }
                if (patchProperty.Property == nameof(ClubStaffEntity.ActiveStatus) && Enum.TryParse<ActiveStatus>(patchProperty.Value?.ToString(), out var status))
                {
                    adUser.AccountEnabled = status == ActiveStatus.Active;
                    continue;
                }
                if (patchProperty.Property == nameof(ClubStaffEntity.BusinessPhoneNumber))
                {
                    adUser.BusinessPhones = [patchProperty.Value?.ToString()];
                    continue;
                }
                // Skip properties not found or mapped
                if (!_propertiesMapping.TryGetValue(patchProperty.Property, out var userProperty))
                {
                    continue;
                }
                // For all others, add new entry into list of properties to update for User
                userUpdates.Add(new() { Property = userProperty, Value = patchProperty.Value });
            }

            adUser.Patch(userUpdates);
            return adUser;
        }
    }
}
