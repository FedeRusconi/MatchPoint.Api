using Bogus;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;

namespace MatchPoint.Api.Tests.Shared.AccessControlService.Helpers
{
    public class CustomRoleBuilder
    {
        private readonly CustomRole _customRole;

        /// <summary>
        /// Instantiate a random <see cref="CustomRole"/>
        /// </summary>
        public CustomRoleBuilder()
        {
            _customRole = new Faker<CustomRole>()
                .RuleFor(r => r.Id, Guid.NewGuid)
                .RuleFor(r => r.Name, f => f.PickRandom("Team Admin", "Supervisor", "Manager", "Coach", "Club President"))
                .RuleFor(r => r.Capabilities, [])
                .Generate();
            Random rd = new();
            foreach(var feature in Enum.GetValues<RoleCapabilityFeature>())
            {
                _customRole.Capabilities.Add(new() { Feature = feature, Action = (RoleCapabilityAction)rd.Next(3)});
            }    
        }

        /// <summary>
        /// Set a specific Id to the <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="CustomRoleBuilder"/>. </returns>
        public CustomRoleBuilder WithId(Guid id)
        {
            _customRole.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="CustomRole"/>.
        /// </summary>
        /// <returns> This <see cref="CustomRoleBuilder"/>. </returns>
        public CustomRoleBuilder WithDefaultId()
        {
            _customRole.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="CustomRoleBuilder"/>. </returns>
        public CustomRoleBuilder WithName(string name)
        {
            _customRole.Name = name;
            return this;
        }

        /// <summary>
        /// Set specific capabilities to the <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="capabilities"> A List of <see cref="RoleCapability"/> to use. </param>
        /// <returns> This <see cref="CustomRoleBuilder"/>. </returns>
        public CustomRoleBuilder WithCapabilities(List<RoleCapability> capabilities)
        {
            _customRole.Capabilities = capabilities;
            return this;
        }

        /// <summary>
        /// Set a specific ActiveStatus to the <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="activeStatus"> The <see cref="ActiveStatus"/> to use. </param>
        /// <returns> This <see cref="CustomRoleBuilder"/>. </returns>
        public CustomRoleBuilder WithActiveStatus(ActiveStatus activeStatus)
        {
            _customRole.ActiveStatus = activeStatus;
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="CustomRole"/>.
        /// </summary>
        /// <returns> The built <see cref="CustomRole"/>. </returns>
        public CustomRole Build()
        {
            return _customRole;
        }
    }
}
