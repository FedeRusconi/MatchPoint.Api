using Bogus;
using Microsoft.Graph.Models;

namespace MatchPoint.Api.Tests.Shared.ClubService.Helpers
{
    public class AzureAdUserBuilder
    {
        private readonly User _user;

        /// <summary>
        /// Instantiate a random <see cref="User"/>
        /// </summary>
        public AzureAdUserBuilder()
        {
            // Use Bogus to initialize the entity with random data
            var passwordProfile = new Faker<PasswordProfile>()
                .RuleFor(p => p.Password, f => f.Internet.Password(prefix: "MatchTest_"))
                .Generate();
            var objectIdentity = new Faker<ObjectIdentity>()
                .RuleFor(i => i.SignInType, "emailAddress")
                .RuleFor(i => i.Issuer, "matchpointdev.onmicrosoft.com")
                .RuleFor(i => i.IssuerAssignedId, f => f.Person.Email)
                .Generate();

            _user = new Faker<User>()
                .RuleFor(c => c.Id, Guid.NewGuid().ToString())
                .RuleFor(c => c.GivenName, f => f.Person.FirstName)
                .RuleFor(c => c.Surname, f => f.Person.LastName)
                .RuleFor(c => c.PasswordProfile, passwordProfile)
                .RuleFor(c => c.Identities, [objectIdentity])
                .RuleFor(c => c.CompanyName, f => f.Person.Company.Name)
                .RuleFor(c => c.JobTitle, f => f.Name.JobTitle())
                .RuleFor(c => c.AccountEnabled, true)
                .Generate();
            _user.DisplayName = $"{_user.GivenName} {_user.Surname}";
            _user.MailNickname = $"{_user.GivenName?.Replace(" ", string.Empty)}.{_user.Surname?.Replace(" ", string.Empty)}";
            _user.UserPrincipalName = $"{objectIdentity.IssuerAssignedId!.Replace("@", "_")}@matchpointdev.onmicrosoft.com";
        }

        /// <summary>
        /// Set a specific Id to the <see cref="User"/>.
        /// </summary>
        /// <param name="id"> The id to use. </param>
        /// <returns> This <see cref="AzureAdUserBuilder"/>. </returns>
        public AzureAdUserBuilder WithId(Guid id)
        {
            _user.Id = id.ToString();
            return this;
        }

        /// <summary>
        /// Set the default Id to the <see cref="User"/>.
        /// </summary>
        /// <returns> This <see cref="AzureAdUserBuilder"/>. </returns>
        public AzureAdUserBuilder WithDefaultId()
        {
            _user.Id = default;
            return this;
        }

        /// <summary>
        /// Set a specific manager of type <see cref="User"/> to the <see cref="User"/>.
        /// </summary>
        /// <param name="manager"> 
        /// The manager to assign. If null provided, a random user is generated as the manager.
        /// Default is null.
        /// </param>
        /// <returns> This <see cref="AzureAdUserBuilder"/>. </returns>
        public AzureAdUserBuilder WithManager(User? manager = null)
        {
            manager ??= new AzureAdUserBuilder().Build();
            _user.Manager = manager;
            return this;
        }

        /// <summary>
        /// Set the address-related properties for the <see cref="User"/>.
        /// </summary>
        /// <returns> This <see cref="AzureAdUserBuilder"/>. </returns>
        public AzureAdUserBuilder WithAddress()
        {
            var faker = new Faker();
            _user.StreetAddress = faker.Address.StreetAddress();
            _user.City = faker.Address.City();
            _user.State = faker.Address.State();
            _user.PostalCode = faker.Address.ZipCode();
            _user.Country = faker.Address.Country();
            return this;
        }

        /// <summary>
        /// Set the phone number-related properties for the <see cref="User"/>.
        /// </summary>
        /// <returns> This <see cref="AzureAdUserBuilder"/>. </returns>
        public AzureAdUserBuilder WithPhoneNumbers()
        {
            var faker = new Faker();
            _user.MobilePhone = faker.Person.Phone;
            _user.BusinessPhones = [faker.Phone.PhoneNumber()];
            return this;
        }

        /// <summary>
        /// Set the employment-related date properties for the <see cref="User"/>.
        /// </summary>
        /// <returns> This <see cref="AzureAdUserBuilder"/>. </returns>
        public AzureAdUserBuilder WithEmploymentDates()
        {
            var faker = new Faker();
            _user.EmployeeHireDate = faker.Date.PastOffset();
            _user.EmployeeLeaveDateTime = faker.Date.PastOffset();
            return this;
        }

        /// <summary>
        /// Complete the build operation for a random <see cref="User"/>.
        /// </summary>
        /// <returns> The built <see cref="User"/>. </returns>
        public User Build()
        {
            return _user;
        }
    }
}
