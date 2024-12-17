using Bogus;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class ClubBuilder
    {
        private readonly Club _club;

        /// <summary>
        /// Instantiate a random <see cref="Club"/>
        /// </summary>
        public ClubBuilder()
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

            _club = new Faker<Club>()
                .RuleFor(c => c.Id, Guid.NewGuid)
                .RuleFor(c => c.Name, f => f.Company.CompanyName())
                .RuleFor(c => c.TaxId, f => f.Random.AlphaNumeric(10))
                .RuleFor(c => c.Address, f => addressGenerator.Generate())
                .RuleFor(c => c.Email, f => f.Person.Email)
                .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                .Generate();
        }

        /// <summary>
        /// Set a specific Id to the <see cref="Club"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="ClubBuilder"/>. </returns>
        public ClubBuilder WithId(Guid id)
        {
            _club.Id = id;
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="Club"/>.
        /// </summary>
        /// <returns> This <see cref="ClubBuilder"/>. </returns>
        public ClubBuilder WithDefaultId()
        {
            _club.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific Name to the <see cref="Club"/>.
        /// </summary>
        /// <param name="name"> The name to use. </param>
        /// <returns> This <see cref="ClubBuilder"/>. </returns>
        public ClubBuilder WithName(string name)
        {
            _club.Name = name;
            return this;
        }

        /// <summary>
        /// Set a specific Email to the <see cref="Club"/>.
        /// </summary>
        /// <param name="email"> The email to use. </param>
        /// <returns> This <see cref="ClubBuilder"/>. </returns>
        public ClubBuilder WithEmail(string email)
        {
            _club.Email = email;
            return this;
        }

        /// <summary>
        /// Set a specific Address to the <see cref="Club"/>.
        /// </summary>
        /// <param name="address"> The <see cref="Address"/> to use. </param>
        /// <returns> This <see cref="ClubBuilder"/>. </returns>
        public ClubBuilder WithAddress(Address address)
        {
            _club.Address = address;
            return this;
        }

        /// <summary>
        /// Set a specific ActiveStatus to the <see cref="Club"/>.
        /// </summary>
        /// <param name="activeStatus"> The <see cref="ActiveStatus"/> to use. </param>
        /// <returns> This <see cref="ClubBuilder"/>. </returns>
        public ClubBuilder WithActiveStatus(ActiveStatus activeStatus)
        {
            _club.ActiveStatus = activeStatus;
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="Club"/>.
        /// </summary>
        /// <returns> The built <see cref="Club"/>. </returns>
        public Club Build()
        {
            return _club;
        }
    }
}
