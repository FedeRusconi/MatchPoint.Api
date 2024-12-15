using Bogus;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;
using MatchPoint.ClubService.Entities;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class ClubEntityBuilder
    {
        private readonly ClubEntity _entity;

        /// <summary>
        /// Instantiate a random <see cref="ClubEntity"/>
        /// </summary>
        public ClubEntityBuilder()
        {
            // Use Bogus to initialize the entity with random data
            var addressGenerator = new Faker<Address>()
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.State, f => f.Address.State())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.Country, new Faker<Country>()
                    .RuleFor(c => c.Code, f => f.Address.CountryCode())
                    .RuleFor(c => c.Name, f => f.Address.Country()));

            _entity = new Faker<ClubEntity>()
                .RuleFor(c => c.Id, Guid.NewGuid)
                .RuleFor(c => c.Name, f => f.Company.CompanyName())
                .RuleFor(c => c.TaxId, f => f.Random.AlphaNumeric(10))
                .RuleFor(c => c.Address, f => addressGenerator.Generate())
                .RuleFor(c => c.Email, f => f.Person.Email)
                .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                .Generate();
        }

        /// <summary>
        /// Set a specific Id to the <see cref="ClubEntity"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubEntityBuilder"/>. </returns>
        public ClubEntityBuilder WithId(Guid id)
        {
            _entity.Id = id;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="ClubEntity"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="ClubEntityBuilder"/>. </returns>
        public ClubEntityBuilder WithName(string name)
        {
            _entity.Name = name;
            return this;
        }

        /// <summary>
        /// Set a specific Email to the <see cref="ClubEntity"/>.
        /// </summary>
        /// <param name="email"> The email to use. </param>
        /// <returns> This <see cref="ClubEntityBuilder"/>. </returns>
        public ClubEntityBuilder WithEmail(string email)
        {
            _entity.Email = email;
            return this;
        }

        /// <summary>
        /// Set a specific Address to the <see cref="ClubEntity"/>.
        /// </summary>
        /// <param name="address"> The <see cref="Address"/> to use. </param>
        /// <returns> This <see cref="ClubEntityBuilder"/>. </returns>
        public ClubEntityBuilder WithAddress(Address address)
        {
            _entity.Address = address;
            return this;
        }

        /// <summary>
        /// Set a specific ActiveStatus to the <see cref="ClubEntity"/>.
        /// </summary>
        /// <param name="activeStatus"> The <see cref="ActiveStatus"/> to use. </param>
        /// <returns> This <see cref="ClubEntityBuilder"/>. </returns>
        public ClubEntityBuilder WithActiveStatus(ActiveStatus activeStatus)
        {
            _entity.ActiveStatus = activeStatus;
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="ClubEntity"/>.
        /// </summary>
        /// <returns> The built <see cref="ClubEntity"/>. </returns>
        public ClubEntity Build()
        {
            return _entity;
        }
    }
}
