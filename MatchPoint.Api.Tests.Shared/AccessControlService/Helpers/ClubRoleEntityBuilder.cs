using Bogus;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Shared.Common.Enums;

namespace MatchPoint.Api.Tests.Shared.AccessControlService.Helpers
{
    public class ClubRoleEntityBuilder
    {
        private readonly ClubRoleEntity _clubRoleEntity;

        /// <summary>
        /// Instantiate a random <see cref="ClubRoleEntity"/>
        /// </summary>
        public ClubRoleEntityBuilder()
        {
            _clubRoleEntity = new Faker<ClubRoleEntity>()
                .RuleFor(r => r.Id, Guid.NewGuid)
                .RuleFor(r => r.Name, f => f.PickRandom("Team Admin", "Supervisor", "Manager", "Coach", "Club President"))
                .RuleFor(r => r.Capabilities, [])
                .Generate();
            Random rd = new();
            foreach (var feature in Enum.GetValues<RoleCapabilityFeature>())
            {
                _clubRoleEntity.Capabilities.Add(new() { Feature = feature, Action = (RoleCapabilityAction)rd.Next(3) });
            }
        }

        /// <summary>
        /// Set a specific Id to the <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubRoleEntityBuilder"/>. </returns>
        public ClubRoleEntityBuilder WithId(Guid id)
        {
            _clubRoleEntity.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <returns> This <see cref="ClubRoleEntityBuilder"/>. </returns>
        public ClubRoleEntityBuilder WithDefaultId()
        {
            _clubRoleEntity.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="ClubRoleEntityBuilder"/>. </returns>
        public ClubRoleEntityBuilder WithName(string name)
        {
            _clubRoleEntity.Name = name;
            return this;
        }

        /// <summary>
        /// Set specific capabilities to the <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <param name="capabilities"> A List of <see cref="RoleCapability"/> to use. </param>
        /// <returns> This <see cref="ClubRoleEntityBuilder"/>. </returns>
        public ClubRoleEntityBuilder WithCapabilities(List<RoleCapability> capabilities)
        {
            _clubRoleEntity.Capabilities = capabilities;
            return this;
        }

        /// <summary>
        /// Set a specific ActiveStatus to the <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <param name="activeStatus"> The <see cref="ActiveStatus"/> to use. </param>
        /// <returns> This <see cref="ClubRoleEntityBuilder"/>. </returns>
        public ClubRoleEntityBuilder WithActiveStatus(ActiveStatus activeStatus)
        {
            _clubRoleEntity.ActiveStatus = activeStatus;
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="ClubRoleEntity"/>.
        /// </summary>
        /// <returns> The built <see cref="ClubRoleEntity"/>. </returns>
        public ClubRoleEntity Build()
        {
            return _clubRoleEntity;
        }
    }
}
