using System.Reflection;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Mappers;

namespace MatchPoint.ClubService.Tests.Unit.Mappers
{
    [TestClass]
    public class ClubMemberMapperTests
    {
        // This is used to ensure no properties are forgotten
        private readonly string[] expectedClubMemberEntityProperties = [
            nameof(ClubMemberEntity.Id), nameof(ClubMemberEntity.FirstName), nameof(ClubMemberEntity.LastName), 
            nameof(ClubMemberEntity.Photo)];
        private readonly string[] expectedClubMemberDtoProperties = [
            nameof(ClubMember.Id), nameof(ClubMember.FirstName), nameof(ClubMember.LastName),
            nameof(ClubMember.Photo)];

        #region To ClubMemberEntity

        [TestMethod]
        public void ToClubMemberEntity_FromClubMemberDto_AllExpectedPropertiesShouldBeSet()
        {
            #region Arrange
            ClubMember clubMemberDto = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };
            #endregion

            #region Act
            ClubMemberEntity result = clubMemberDto.ToClubMemberEntity();
            #endregion

            #region Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(properties.All(prop => expectedClubMemberEntityProperties.Contains(prop.Name)));
            #endregion
        }

        [TestMethod]
        public void ToClubMemberEntity_FromClubMemberDto_ValidParameter_ShouldReturnClubMemberEntity()
        {
            #region Arrange
            ClubMember clubMemberDto = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };
            #endregion

            #region Act
            ClubMemberEntity result = clubMemberDto.ToClubMemberEntity();
            #endregion

            #region Assert
            Assert.AreEqual(clubMemberDto.Id, result.Id);
            Assert.AreEqual(clubMemberDto.FirstName, result.FirstName);
            Assert.AreEqual(clubMemberDto.LastName, result.LastName);
            Assert.AreEqual(clubMemberDto.Photo, result.Photo);
            #endregion
        }

        [TestMethod]
        public void ToClubMemberEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfClubMemberEntity()
        {
            #region Arrange
            ClubMember clubMember1 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 1",
                LastName = "Last 1"
            };
            ClubMember clubMember2 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 2",
                LastName = "Last 2"
            };
            ClubMember clubMember3 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 3",
                LastName = "Last 3"
            };
            List<ClubMember> clubMembers = [clubMember1, clubMember2, clubMember3];
            #endregion

            #region  Act
            IEnumerable<ClubMemberEntity> result = clubMembers.ToClubMemberEntityEnumerable();
            #endregion

            #region Assert
            Assert.AreEqual(clubMembers.Count, result.Count());
            Assert.AreEqual(clubMembers[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubMembers[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubMembers[2].Id, result.ElementAt(2).Id);
            #endregion
        }

        [TestMethod]
        public void ToClubMemberEntityEnumerable_NullClubMember_ShouldThrowNullReferenceException()
        {
            #region  Arrange
            ClubMember clubMember1 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 1",
                LastName = "Last 1"
            };
            ClubMember clubMember2 = null!;
            ClubMember clubMember3 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 3",
                LastName = "Last 3"
            };
            List<ClubMember> clubMembers = [clubMember1, clubMember2, clubMember3];
            #endregion

            #region Act & Assert
            var result = clubMembers.ToClubMemberEntityEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
            #endregion
        }

        #endregion

        #region To ClubMemberDto

        [TestMethod]
        public void ToClubMemberDto_FromClubMemberEntity_AllExpectedPropertiesShouldBeSet()
        {
            #region Arrange
            ClubMemberEntity clubMemberEntity = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };
            #endregion

            #region Act
            ClubMember result = clubMemberEntity.ToClubMemberDto();
            #endregion

            #region Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(properties.All(prop => expectedClubMemberDtoProperties.Contains(prop.Name)));
            #endregion
        }

        [TestMethod]
        public void ToClubMemberDto_FromClubMemberEntity_ValidParameter_ShouldReturnClubMemberDto()
        {
            #region Arrange
            ClubMemberEntity clubMemberEntity = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };
            #endregion

            #region Act
            ClubMember result = clubMemberEntity.ToClubMemberDto();
            #endregion

            #region Assert
            Assert.AreEqual(clubMemberEntity.Id, result.Id);
            Assert.AreEqual(clubMemberEntity.FirstName, result.FirstName);
            Assert.AreEqual(clubMemberEntity.LastName, result.LastName);
            Assert.AreEqual(clubMemberEntity.Photo, result.Photo);
            #endregion
        }

        [TestMethod]
        public void ToClubMemberDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfClubMemberDto()
        {
            #region Arrange
            ClubMemberEntity clubMemberEntity1 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 1",
                LastName = "Last 1"
            };
            ClubMemberEntity clubMemberEntity2 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 2",
                LastName = "Last 2"
            };
            ClubMemberEntity clubMemberEntity3 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 3",
                LastName = "Last 3"
            };
            List<ClubMemberEntity> clubMemberEntities = [clubMemberEntity1, clubMemberEntity2, clubMemberEntity3];
            #endregion

            #region  Act
            IEnumerable<ClubMember> result = clubMemberEntities.ToClubMemberDtoEnumerable();
            #endregion

            #region Assert
            Assert.AreEqual(clubMemberEntities.Count, result.Count());
            Assert.AreEqual(clubMemberEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubMemberEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubMemberEntities[2].Id, result.ElementAt(2).Id);
            #endregion
        }

        [TestMethod]
        public void ToClubMemberDtoEnumerable_NullClubMember_ShouldThrowNullReferenceException()
        {
            #region  Arrange
            ClubMemberEntity clubMemberEntity1 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 1",
                LastName = "Last 1"
            };
            ClubMemberEntity clubMemberEntity2 = null!;
            ClubMemberEntity clubMemberEntity3 = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First 3",
                LastName = "Last 3"
            };
            List<ClubMemberEntity> clubMemberEntities = [clubMemberEntity1, clubMemberEntity2, clubMemberEntity3];
            #endregion

            #region Act & Assert
            var result = clubMemberEntities.ToClubMemberDtoEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
            #endregion
        }

        #endregion
    }
}
