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
        // This is used to ensure no properties are forgotten
        private readonly string[] expectedClubEntityProperties = [
            nameof(ClubEntity.Id), nameof(ClubEntity.Email), nameof(ClubEntity.Name), nameof(ClubEntity.PhoneNumber),
            nameof(ClubEntity.Address), nameof(ClubEntity.Logo), nameof(ClubEntity.ActiveStatus), nameof(ClubEntity.TaxId),
            nameof(ClubEntity.OpeningTimes), nameof(ClubEntity.Courts), nameof(ClubEntity.SocialMedia), nameof(ClubEntity.Staff),
            nameof(ClubEntity.Members), nameof(ClubEntity.CreatedBy), nameof(ClubEntity.CreatedOnUtc),
            nameof(ClubEntity.ModifiedBy), nameof(ClubEntity.ModifiedOnUtc), nameof(ClubEntity.TimezoneId)];
        private readonly string[] expectedClubDtoProperties = [
            nameof(Club.Id), nameof(Club.Email), nameof(Club.Name), nameof(Club.PhoneNumber),
            nameof(Club.Address), nameof(Club.Logo), nameof(Club.ActiveStatus), nameof(Club.TaxId),
            nameof(Club.OpeningTimes), nameof(Club.Courts), nameof(Club.SocialMedia), nameof(Club.Staff),
            nameof(Club.Members), nameof(Club.CreatedBy), nameof(Club.CreatedOnUtc),
            nameof(Club.ModifiedBy), nameof(Club.ModifiedOnUtc), nameof(Club.TimezoneId)];

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
            #region Arrange
            Club clubDto = _clubBuilder.Build();
            #endregion

            #region Act
            ClubEntity result = clubDto.ToClubEntity();
            #endregion

            #region Assert
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
            ClubEntity result = club.ToClubEntity();
            #endregion

            #region Assert
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
            var result = clubs.ToClubEntityEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
            #endregion
        }

        #endregion
        #region To ClubDto

        [TestMethod]
        public void ToClubDto_FromClubEntity_AllExpectedPropertiesShouldBeSet()
        {
            #region Arrange
            ClubEntity clubEntity = _clubEntityBuilder.Build();
            #endregion

            #region Act
            Club result = clubEntity.ToClubDto();
            #endregion

            #region Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(properties.All(prop => expectedClubDtoProperties.Contains(prop.Name)));
            #endregion
        }

        [TestMethod]
        public void ToClubDto_FromClubEntity_ValidParameter_ShouldReturnClubDto()
        {
            #region Arrange
            ClubEntity clubEntity = _clubEntityBuilder.Build();
            #endregion

            #region Act
            Club result = clubEntity.ToClubDto();
            #endregion

            #region Assert
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
            #endregion
        }

        [TestMethod]
        public void ToClubDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfClubDto()
        {
            #region Arrange
            // Add two more builders
            var clubEntityBuilder2 = new ClubEntityBuilder();
            var clubEntityBuilder3 = new ClubEntityBuilder();
            List<ClubEntity> clubEntities = [_clubEntityBuilder.Build(), clubEntityBuilder2.Build(), clubEntityBuilder3.Build()];
            #endregion

            #region  Act
            IEnumerable<Club> result = clubEntities.ToClubDtoEnumerable();
            #endregion

            #region Assert
            Assert.AreEqual(clubEntities.Count, result.Count());
            Assert.AreEqual(clubEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubEntities[2].Id, result.ElementAt(2).Id);
            #endregion
        }

        [TestMethod]
        public void ToClubDtoEnumerable_NullClub_ShouldThrowNullReferenceException()
        {
            #region  Arrange
            // Add one more builder
            var clubEntityBuilder2 = new ClubEntityBuilder();
            ClubEntity nullClubEntity = null!;
            List<ClubEntity> clubEntities = [_clubEntityBuilder.Build(), nullClubEntity, clubEntityBuilder2.Build()];
            #endregion

            #region Act & Assert
            var result = clubEntities.ToClubDtoEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
            #endregion
        }

        #endregion
    }
}
