using Bogus;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class ClubStaffEntityBuilder
    {
        private readonly ClubStaffEntity _clubStaff;

        /// <summary>
        /// Instantiate a random <see cref="ClubStaffEntity"/>
        /// </summary>
        public ClubStaffEntityBuilder()
        {
            _clubStaff = new Faker<ClubStaffEntity>()
                .RuleFor(c => c.Id, Guid.NewGuid)
                .RuleFor(c => c.ClubId, Guid.NewGuid)
                .RuleFor(c => c.RoleId, Guid.NewGuid())
                .RuleFor(c => c.Email, f => f.Person.Email)
                .RuleFor(c => c.RoleName, f => f.Internet.UserAgent())
                .Generate();
        }

        /// <summary>
        /// Set a specific Id to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithId(Guid id)
        {
            _clubStaff.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithDefaultId()
        {
            _clubStaff.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific ClubId to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithClubId(Guid id)
        {
            _clubStaff.ClubId = id;
            return this;
        }

        /// <summary>
        /// Set the default ClubId to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithDefaultClubId()
        {
            _clubStaff.ClubId = default;
            return this;
        }

        /// <summary>
        /// Set a specific RoleId to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithRoleId(Guid id)
        {
            _clubStaff.RoleId = id;
            return this;
        }

        /// <summary>
        /// Set the default RoleId to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithDefaultRoleId()
        {
            _clubStaff.RoleId = default;
            return this;
        }

        /// <summary>
        /// Set a specific RoleName to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithRoleName(string name)
        {
            _clubStaff.RoleName = name;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <param name="firstName"> 
        /// The first name to use. If value is null, a random name is used.
        /// Default is null.
        /// </param>
        /// <param name="lastName"> 
        /// The last name to use. If value is null, a random name is used.
        /// Default is null.
        /// </param>
        /// <returns> This <see cref="ClubStaffEntityBuilder"/>. </returns>
        public ClubStaffEntityBuilder WithName(string? firstName = null, string? lastName = null)
        {
            var faker = new Faker();
            _clubStaff.FirstName = firstName ?? faker.Person.FirstName;
            _clubStaff.LastName = lastName ?? faker.Person.LastName;
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="ClubStaffEntity"/>.
        /// </summary>
        /// <returns> The built <see cref="ClubStaffEntity"/>. </returns>
        public ClubStaffEntity Build()
        {
            return _clubStaff;
        }
    }
}
