using System.Reflection;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Mappers;

namespace MatchPoint.ClubService.Tests.Unit.Mappers
{
    [TestClass]
    public class ClubMemberMapperTests
    {
        #region To ClubMemberEntity

        [TestMethod]
        public void ToClubMemberEntity_FromClubMemberDto_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubMember clubMemberDto = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };

            // Act
            ClubMemberEntity result = clubMemberDto.ToClubMemberEntity();

            //Assert
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
        public void ToClubMemberEntity_FromClubMemberDto_ValidParameter_ShouldReturnClubMemberEntity()
        {
            // Arrange
            ClubMember clubMemberDto = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };

            // Act
            ClubMemberEntity result = clubMemberDto.ToClubMemberEntity();

            // Assert
            Assert.AreEqual(clubMemberDto.Id, result.Id);
            Assert.AreEqual(clubMemberDto.FirstName, result.FirstName);
            Assert.AreEqual(clubMemberDto.LastName, result.LastName);
            Assert.AreEqual(clubMemberDto.Photo, result.Photo);
        }

        [TestMethod]
        public void ToClubMemberEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfClubMemberEntity()
        {
            // Arrange
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

            //  Act
            IEnumerable<ClubMemberEntity> result = clubMembers.ToClubMemberEntityEnumerable();

            // Assert
            Assert.AreEqual(clubMembers.Count, result.Count());
            Assert.AreEqual(clubMembers[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubMembers[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubMembers[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubMemberEntityEnumerable_NullClubMember_ShouldThrowNullReferenceException()
        {
            //  Arrange
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

            // Act & Assert
            var result = clubMembers.ToClubMemberEntityEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion

        #region To ClubMemberDto

        [TestMethod]
        public void ToClubMemberDto_FromClubMemberEntity_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubMemberEntity clubMemberEntity = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };

            // Act
            ClubMember result = clubMemberEntity.ToClubMemberDto();

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
        public void ToClubMemberDto_FromClubMemberEntity_ValidParameter_ShouldReturnClubMemberDto()
        {
            // Arrange
            ClubMemberEntity clubMemberEntity = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };

            // Act
            ClubMember result = clubMemberEntity.ToClubMemberDto();

            // Assert
            Assert.AreEqual(clubMemberEntity.Id, result.Id);
            Assert.AreEqual(clubMemberEntity.FirstName, result.FirstName);
            Assert.AreEqual(clubMemberEntity.LastName, result.LastName);
            Assert.AreEqual(clubMemberEntity.Photo, result.Photo);
        }

        [TestMethod]
        public void ToClubMemberDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfClubMemberDto()
        {
            // Arrange
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

            // Act
            IEnumerable<ClubMember> result = clubMemberEntities.ToClubMemberDtoEnumerable();

            // Assert
            Assert.AreEqual(clubMemberEntities.Count, result.Count());
            Assert.AreEqual(clubMemberEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubMemberEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubMemberEntities[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubMemberDtoEnumerable_NullClubMember_ShouldThrowNullReferenceException()
        {
            // Arrange
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

            // Act & Assert
            var result = clubMemberEntities.ToClubMemberDtoEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
    }
}
