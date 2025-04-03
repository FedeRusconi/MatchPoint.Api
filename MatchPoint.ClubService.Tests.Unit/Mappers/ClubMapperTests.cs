using System.Reflection;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Mappers;

namespace MatchPoint.ClubService.Tests.Unit.Mappers
{
    [TestClass]
    public class ClubMapperTests
    {
        private ClubBuilder _clubBuilder = default!;
        private ClubEntityBuilder _clubEntityBuilder = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _clubBuilder = new();
            _clubEntityBuilder = new();
        }

        #region To ClubEntity

        [TestMethod]
        public void ToClubEntity_FromClubDto_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            Club clubDto = _clubBuilder.WithTrackingFields().Build();

            // Act
            ClubEntity result = clubDto.ToClubEntity();

            // Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetValue(result) == default)
                {
                    Assert.Fail($"{prop.Name} has not been set.");
                    break;
                }
            }
        }

        [TestMethod]
        public void ToClubEntity_FromClubDto_ValidParameter_ShouldReturnClubEntity()
        {
            // Arrange
            Club club = _clubBuilder.Build();

            // Act
            ClubEntity result = club.ToClubEntity();

            // Assert
            Assert.AreEqual(club.Id, result.Id);
            Assert.AreEqual(club.Email, result.Email);
            Assert.AreEqual(club.Name, result.Name);
            Assert.AreEqual(club.PhoneNumber, result.PhoneNumber);
            Assert.AreEqual(club.Address, result.Address);
            Assert.AreEqual(club.Logo, result.Logo);
            Assert.AreEqual(club.ActiveStatus, result.ActiveStatus);
            Assert.AreEqual(club.TaxId, result.TaxId);
            Assert.AreEqual(club.OpeningTimes, result.OpeningTimes);
            Assert.AreEqual(club.SocialMedia, result.SocialMedia);
            Assert.AreEqual(club.CreatedBy, result.CreatedBy);
            Assert.AreEqual(club.CreatedOnUtc, result.CreatedOnUtc);
            Assert.AreEqual(club.ModifiedBy, result.ModifiedBy);
            Assert.AreEqual(club.ModifiedOnUtc, result.ModifiedOnUtc);
            Assert.AreEqual(club.TimezoneId, result.TimezoneId);
        }

        [TestMethod]
        public void ToClubEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfClubEntity()
        {
            // Arrange
            // Add two more builders
            var clubBuilder2 = new ClubBuilder();
            var clubBuilder3 = new ClubBuilder();
            List<Club> clubs = [_clubBuilder.Build(), clubBuilder2.Build(), clubBuilder3.Build()];

            //  Act
            IEnumerable<ClubEntity> result = clubs.ToClubEntityEnumerable();

            // Assert
            Assert.AreEqual(clubs.Count, result.Count());
            Assert.AreEqual(clubs[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubs[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubs[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubEntityEnumerable_NullClub_ShouldThrowNullReferenceException()
        {
            //  Arrange
            // Add one more builder
            var clubBuilder2 = new ClubBuilder();
            Club nullClub = null!;
            List<Club> clubs = [_clubBuilder.Build(), nullClub, clubBuilder2.Build()];

            // Act & Assert
            var result = clubs.ToClubEntityEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
        #region To ClubDto

        [TestMethod]
        public void ToClubDto_FromClubEntity_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubEntity clubEntity = _clubEntityBuilder.WithTrackingFields().Build();

            // Act
            Club result = clubEntity.ToClubDto();

            // Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetValue(result) == default)
                {
                    Assert.Fail($"{prop.Name} has not been set.");
                    break;
                }
            }
        }

        [TestMethod]
        public void ToClubDto_FromClubEntity_ValidParameter_ShouldReturnClubDto()
        {
            // Arrange
            ClubEntity clubEntity = _clubEntityBuilder.Build();

            // Act
            Club result = clubEntity.ToClubDto();

            // Assert
            Assert.AreEqual(clubEntity.Id, result.Id);
            Assert.AreEqual(clubEntity.Email, result.Email);
            Assert.AreEqual(clubEntity.Name, result.Name);
            Assert.AreEqual(clubEntity.PhoneNumber, result.PhoneNumber);
            Assert.AreEqual(clubEntity.Address, result.Address);
            Assert.AreEqual(clubEntity.Logo, result.Logo);
            Assert.AreEqual(clubEntity.ActiveStatus, result.ActiveStatus);
            Assert.AreEqual(clubEntity.TaxId, result.TaxId);
            Assert.AreEqual(clubEntity.OpeningTimes, result.OpeningTimes);
            Assert.AreEqual(clubEntity.SocialMedia, result.SocialMedia);
            Assert.AreEqual(clubEntity.CreatedBy, result.CreatedBy);
            Assert.AreEqual(clubEntity.CreatedOnUtc, result.CreatedOnUtc);
            Assert.AreEqual(clubEntity.ModifiedBy, result.ModifiedBy);
            Assert.AreEqual(clubEntity.ModifiedOnUtc, result.ModifiedOnUtc);
            Assert.AreEqual(clubEntity.TimezoneId, result.TimezoneId);
        }

        [TestMethod]
        public void ToClubDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfClubDto()
        {
            // Arrange
            // Add two more builders
            var clubEntityBuilder2 = new ClubEntityBuilder();
            var clubEntityBuilder3 = new ClubEntityBuilder();
            List<ClubEntity> clubEntities = 
            [
                _clubEntityBuilder.Build(), clubEntityBuilder2.Build(), clubEntityBuilder3.Build()
            ];

            //  Act
            IEnumerable<Club> result = clubEntities.ToClubDtoEnumerable();

            // Assert
            Assert.AreEqual(clubEntities.Count, result.Count());
            Assert.AreEqual(clubEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubEntities[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubDtoEnumerable_NullClub_ShouldThrowNullReferenceException()
        {
            //  Arrange
            // Add one more builder
            var clubEntityBuilder2 = new ClubEntityBuilder();
            ClubEntity nullClubEntity = null!;
            List<ClubEntity> clubEntities = [_clubEntityBuilder.Build(), nullClubEntity, clubEntityBuilder2.Build()];

            // Act & Assert
            var result = clubEntities.ToClubDtoEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
    }
}
