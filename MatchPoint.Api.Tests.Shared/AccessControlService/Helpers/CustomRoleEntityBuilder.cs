using Bogus;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.Api.Shared.AccessControlService.Enums;
using MatchPoint.Api.Shared.AccessControlService.Models;

namespace MatchPoint.Api.Tests.Shared.AccessControlService.Helpers
{
    public class CustomRoleEntityBuilder
    {
        private readonly CustomRoleEntity _customRoleEntity;

        /// <summary>
        /// Instantiate a random <see cref="CustomRoleEntity"/>
        /// </summary>
        public CustomRoleEntityBuilder()
        {
            _customRoleEntity = new Faker<CustomRoleEntity>()
                .RuleFor(r => r.Id, Guid.NewGuid)
                .RuleFor(r => r.Name, f => f.PickRandom("Team Admin", "Supervisor", "Manager", "Coach", "Club President"))
                .RuleFor(r => r.Capabilities, [])
                .Generate();
            Random rd = new();
            foreach (var feature in Enum.GetValues<RoleCapabilityFeature>())
            {
                _customRoleEntity.Capabilities.Add(new() { Feature = feature, Action = (RoleCapabilityAction)rd.Next(3) });
            }
        }

        /// <summary>
        /// Set a specific Id to the <see cref="CustomRoleEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="CustomRoleEntityBuilder"/>. </returns>
        public CustomRoleEntityBuilder WithId(Guid id)
        {
            _customRoleEntity.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="CustomRoleEntity"/>.
        /// </summary>
        /// <returns> This <see cref="CustomRoleEntityBuilder"/>. </returns>
        public CustomRoleEntityBuilder WithDefaultId()
        {
            _customRoleEntity.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="CustomRoleEntity"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="CustomRoleEntityBuilder"/>. </returns>
        public CustomRoleEntityBuilder WithName(string name)
        {
            _customRoleEntity.Name = name;
            return this;
        }

        /// <summary>
        /// Set specific capabilities to the <see cref="CustomRoleEntity"/>.
        /// </summary>
        /// <param name="capabilities"> A List of <see cref="RoleCapability"/> to use. </param>
        /// <returns> This <see cref="CustomRoleEntityBuilder"/>. </returns>
        public CustomRoleEntityBuilder WithCapabilities(List<RoleCapability> capabilities)
        {
            _customRoleEntity.Capabilities = capabilities;
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="CustomRoleEntity"/>.
        /// </summary>
        /// <returns> The built <see cref="CustomRoleEntity"/>. </returns>
        public CustomRoleEntity Build()
        {
            return _customRoleEntity;
        }
    }
}
