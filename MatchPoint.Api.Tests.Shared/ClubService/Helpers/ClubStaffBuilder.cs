using Bogus;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class ClubStaffBuilder
    {
        private readonly ClubStaff _clubStaff;

        /// <summary>
        /// Instantiate a random <see cref="ClubStaff"/>
        /// </summary>
        public ClubStaffBuilder()
        {
            var addressGenerator = new Faker<Address>()
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.State, f => f.Address.State())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.Country, new Faker<Country>()
                    .RuleFor(c => c.Code, f => f.Address.CountryCode())
                    .RuleFor(c => c.Name, f => f.Address.Country()));

            _clubStaff = new Faker<ClubStaff>()
                .RuleFor(c => c.Id, Guid.NewGuid)
                .RuleFor(c => c.ClubId, Guid.NewGuid)
                .RuleFor(c => c.Email, f => f.Person.Email)
                .RuleFor(c => c.FirstName, f => f.Person.FirstName)
                .RuleFor(c => c.LastName, f => f.Person.LastName)
                .RuleFor(c => c.JobTitle, f => f.Name.JobTitle())
                .RuleFor(c => c.PhoneNumber, f => f.Person.Phone)
                .RuleFor(c => c.BusinessPhoneNumber, f => f.Person.Phone)
                .RuleFor(c => c.Photo, f => f.Random.AlphaNumeric(10))
                .RuleFor(c => c.RoleId, Guid.NewGuid())
                .RuleFor(c => c.RoleName, f => f.Internet.UserAgent())
                .RuleFor(c => c.ActiveStatus, ActiveStatus.Active)
                .RuleFor(c => c.Address, addressGenerator.Generate())
                .RuleFor(c => c.ManagerId, Guid.NewGuid())
                .RuleFor(c => c.HiredOnUtc, f => f.Date.Past())
                .RuleFor(c => c.LeftOnUtc, f => f.Date.Past())
                .Generate();
        }

        /// <summary>
        /// Set a specific Id to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithId(Guid id)
        {
            _clubStaff.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithDefaultId()
        {
            _clubStaff.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific ClubId to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithClubId(Guid id)
        {
            _clubStaff.ClubId = id;
            return this;
        }

        /// <summary>
        /// Set the default ClubId to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithDefaultClubId()
        {
            _clubStaff.ClubId = default;
            return this;
        }

        /// <summary>
        /// Set a specific RoleId to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithRoleId(Guid id)
        {
            _clubStaff.RoleId = id;
            return this;
        }

        /// <summary>
        /// Set the default RoleId to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithDefaultRoleId()
        {
            _clubStaff.RoleId = default;
            return this;
        }

        /// <summary>
        /// Set a specific RoleName to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithRoleName(string name)
        {
            _clubStaff.RoleName = name;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="ClubStaff"/>.
        /// </summary>
        /// <param name="firstName"> 
        /// The first name to use. If value is null, a random name is used.
        /// Default is null.
        /// </param>
        /// <param name="lastName"> 
        /// The last name to use. If value is null, a random name is used.
        /// Default is null.
        /// </param>
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithName(string? firstName = null, string? lastName = null)
        {
            var faker = new Faker();
            _clubStaff.FirstName = firstName ?? faker.Person.FirstName;
            _clubStaff.LastName = lastName ?? faker.Person.LastName;
            return this;
        }

        /// <summary>
        /// Set the tracking fields (crated, modified) for the <see cref="ClubStaff"/>.
        /// <returns> This <see cref="ClubStaffBuilder"/>. </returns>
        public ClubStaffBuilder WithTrackingFields()
        {
            var faker = new Faker();
            _clubStaff.CreatedBy = Guid.NewGuid();
            _clubStaff.CreatedOnUtc = faker.Date.Past();
            _clubStaff.ModifiedBy = Guid.NewGuid();
            _clubStaff.ModifiedOnUtc = faker.Date.Past();
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="ClubStaff"/>.
        /// </summary>
        /// <returns> The built <see cref="ClubStaff"/>. </returns>
        public ClubStaff Build()
        {
            return _clubStaff;
        }
    }
}
