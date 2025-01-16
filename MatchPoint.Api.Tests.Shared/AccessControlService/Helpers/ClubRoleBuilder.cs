using Bogus;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;

namespace MatchPoint.Api.Tests.Shared.AccessControlService.Helpers
{
    public class ClubRoleBuilder
    {
        private readonly ClubRole _clubRole;

        /// <summary>
        /// Instantiate a random <see cref="ClubRole"/>
        /// </summary>
        public ClubRoleBuilder()
        {
            _clubRole = new Faker<ClubRole>()
                .RuleFor(r => r.Id, Guid.NewGuid)
                .RuleFor(r => r.Name, f => f.PickRandom("Team Admin", "Supervisor", "Manager", "Coach", "Club President"))
                .RuleFor(r => r.Capabilities, [])
                .Generate();
            Random rd = new();
            foreach (var feature in Enum.GetValues<RoleCapabilityFeature>())
            {
                _clubRole.Capabilities.Add(new() { Feature = feature, Action = (RoleCapabilityAction)rd.Next(3) });
            }
        }

        /// <summary>
        /// Set a specific Id to the <see cref="ClubRole"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubRoleBuilder"/>. </returns>
        public ClubRoleBuilder WithId(Guid id)
        {
            _clubRole.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="ClubRole"/>.
        /// </summary>
        /// <returns> This <see cref="ClubRoleBuilder"/>. </returns>
        public ClubRoleBuilder WithDefaultId()
        {
            _clubRole.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="ClubRole"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="ClubRoleBuilder"/>. </returns>
        public ClubRoleBuilder WithName(string name)
        {
            _clubRole.Name = name;
            return this;
        }

        /// <summary>
        /// Set specific capabilities to the <see cref="ClubRole"/>.
        /// </summary>
        /// <param name="capabilities"> A List of <see cref="RoleCapability"/> to use. </param>
        /// <returns> This <see cref="ClubRoleBuilder"/>. </returns>
        public ClubRoleBuilder WithCapabilities(List<RoleCapability> capabilities)
        {
            _clubRole.Capabilities = capabilities;
            return this;
        }

        /// <summary>
        /// Set a specific ActiveStatus to the <see cref="ClubRole"/>.
        /// </summary>
        /// <param name="activeStatus"> The <see cref="ActiveStatus"/> to use. </param>
        /// <returns> This <see cref="ClubRoleBuilder"/>. </returns>
        public ClubRoleBuilder WithActiveStatus(ActiveStatus activeStatus)
        {
            _clubRole.ActiveStatus = activeStatus;
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="ClubRole"/>.
        /// </summary>
        /// <returns> The built <see cref="CustomRole"/>. </returns>
        public ClubRole Build()
        {
            return _clubRole;
        }
    }
}
