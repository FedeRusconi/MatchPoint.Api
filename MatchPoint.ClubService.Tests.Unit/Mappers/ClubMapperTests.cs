using System.Reflection;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Mappers;
using MatchPoint.ClubService.Tests.Unit.Helpers;

namespace MatchPoint.ClubService.Tests.Unit.Mappers
{
    [TestClass]
    public class ClubMapperTests
    {
        // This is used to ensure no properties are forgotten
        private readonly string[] expectedClubEntityProperties = [
            nameof(ClubEntity.Id), nameof(ClubEntity.Email), nameof(ClubEntity.Name), nameof(ClubEntity.PhoneNumber),
            nameof(ClubEntity.Address), nameof(ClubEntity.Logo), nameof(ClubEntity.ActiveStatus), nameof(ClubEntity.TaxId),
            nameof(ClubEntity.OpeningTimes), nameof(ClubEntity.Courts), nameof(ClubEntity.SocialMedia), nameof(ClubEntity.Staff),
            nameof(ClubEntity.Members), nameof(ClubEntity.CreatedBy), nameof(ClubEntity.CreatedOnUtc),
            nameof(ClubEntity.ModifiedBy), nameof(ClubEntity.ModifiedOnUtc), nameof(ClubEntity.TimezoneId)];

        private ClubBuilder _clubBuilder = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _clubBuilder = new();
        }

        [TestMethod]
        public void ToClubEntity_FromClubDto_AllExpectedPropertiesShouldBeSet()
        {
            #region Arrange
            Club clubDto = _clubBuilder.Build();
            #endregion

            #region Act
            // Act
            ClubEntity result = clubDto.ToClubEntity();
            #endregion

            #region Assert
            // Assert all properties are present
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(properties.All(prop => expectedClubEntityProperties.Contains(prop.Name)));
            #endregion
        }

        [TestMethod]
        public void ToClubEntity_FromClubDto_ValidParameter_ShouldReturnClubEntity()
        {
            #region Arrange
            Club club = _clubBuilder.Build();
            #endregion

            #region Act
            ClubEntity clubEntity = club.ToClubEntity();
            #endregion

            #region Assert
            Assert.AreEqual(club.Id, clubEntity.Id);
            Assert.AreEqual(club.Email, clubEntity.Email);
            Assert.AreEqual(club.Name, clubEntity.Name);
            Assert.AreEqual(club.PhoneNumber, clubEntity.PhoneNumber);
            Assert.AreEqual(club.Address, clubEntity.Address);
            Assert.AreEqual(club.Logo, clubEntity.Logo);
            Assert.AreEqual(club.ActiveStatus, clubEntity.ActiveStatus);
            Assert.AreEqual(club.TaxId, clubEntity.TaxId);
            Assert.AreEqual(club.OpeningTimes, clubEntity.OpeningTimes);
            Assert.AreEqual(club.SocialMedia, clubEntity.SocialMedia);
            Assert.AreEqual(club.CreatedBy, clubEntity.CreatedBy);
            Assert.AreEqual(club.CreatedOnUtc, clubEntity.CreatedOnUtc);
            Assert.AreEqual(club.ModifiedBy, clubEntity.ModifiedBy);
            Assert.AreEqual(club.ModifiedOnUtc, clubEntity.ModifiedOnUtc);
            Assert.AreEqual(club.TimezoneId, clubEntity.TimezoneId);
            #endregion
        }

        [TestMethod]
        public void ToClubEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfClubEntity()
        {
            #region Arrange
            // Add two more builders
            var clubBuilder2 = new ClubBuilder();
            var clubBuilder3 = new ClubBuilder();
            List<Club> clubs = [_clubBuilder.Build(), clubBuilder2.Build(), clubBuilder3.Build()];
            #endregion

            #region  Act
            IEnumerable<ClubEntity> result = clubs.ToClubEntityEnumerable();
            #endregion

            #region Assert
            Assert.AreEqual(clubs.Count, result.Count());
            Assert.AreEqual(clubs[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubs[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubs[2].Id, result.ElementAt(2).Id);
            #endregion
        }

        [TestMethod]
        public void ToClubEntityEnumerable_NullClub_ShouldThrowNullReferenceException()
        {
            #region  Arrange
            // Add one more builder
            var clubBuilder2 = new ClubBuilder();
            Club nullClub = null!;
            List<Club> clubs = [_clubBuilder.Build(), nullClub, clubBuilder2.Build()];
            #endregion

            #region Act & Assert
            var convertedClubs = clubs.ToClubEntityEnumerable();
            Assert.ThrowsException<NullReferenceException>(convertedClubs.ToList);
            #endregion
        }
    }
}
