using System.Reflection;
using MatchPoint.Api.Shared.ClubService.Models;
using MatchPoint.Api.Tests.Shared.ClubService.Helpers;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Mappers;

namespace MatchPoint.ClubService.Tests.Unit.Mappers
{
    [TestClass]
    public class ClubStaffMapperTests
    {
        private ClubStaffEntityBuilder _clubStaffEntityBuilder = default!;
        private ClubStaffBuilder _clubStaffBuilder = default!;

        [TestInitialize]
        public void Setup()
        {
            _clubStaffEntityBuilder = new();
            _clubStaffBuilder = new();
        }

        #region To ClubStaffEntity

        [TestMethod]
        public void ToClubStaffEntity_FromClubStaffDto_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubStaff clubStaffDto = _clubStaffBuilder.WithTrackingFields().Build();

            // Act
            ClubStaffEntity result = clubStaffDto.ToClubStaffEntity();

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
        public void ToClubStaffEntity_FromClubStaffDto_ValidParameter_ShouldReturnClubStaffEntity()
        {
            // Arrange
            ClubStaff clubStaffDto = _clubStaffBuilder.WithTrackingFields().Build();

            // Act
            ClubStaffEntity result = clubStaffDto.ToClubStaffEntity();

            // Assert
            Assert.AreEqual(clubStaffDto.Id, result.Id);
            Assert.AreEqual(clubStaffDto.Email, result.Email);
            Assert.AreEqual(clubStaffDto.Photo, result.Photo);
            Assert.AreEqual(clubStaffDto.RoleId, result.RoleId);
            Assert.AreEqual(clubStaffDto.RoleName, result.RoleName);
            Assert.AreEqual(clubStaffDto.ClubId, result.ClubId);
        }

        [TestMethod]
        public void ToClubStaffEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfClubStaffEntity()
        {
            // Arrange
            ClubStaff clubStaff1 = _clubStaffBuilder.WithTrackingFields().Build();
            _clubStaffBuilder = new();
            ClubStaff clubStaff2 = _clubStaffBuilder.WithTrackingFields().Build();
            _clubStaffBuilder = new();
            ClubStaff clubStaff3 = _clubStaffBuilder.WithTrackingFields().Build();
            List<ClubStaff> clubStaffs = [clubStaff1, clubStaff2, clubStaff3];

            // Act
            IEnumerable<ClubStaffEntity> result = clubStaffs.ToClubStaffEntityEnumerable();

            // Assert
            Assert.AreEqual(clubStaffs.Count, result.Count());
            Assert.AreEqual(clubStaffs[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubStaffs[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubStaffs[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubStaffEntityEnumerable_NullClubStaff_ShouldThrowNullReferenceException()
        {
            // Arrange
            ClubStaff clubStaff1 = _clubStaffBuilder.WithTrackingFields().Build();
            ClubStaff clubStaff2 = null!;
            _clubStaffBuilder = new();
            ClubStaff clubStaff3 = _clubStaffBuilder.WithTrackingFields().Build();
            List<ClubStaff> clubStaffs = [clubStaff1, clubStaff2, clubStaff3];

            // Act & Assert
            var result = clubStaffs.ToClubStaffEntityEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
        }

        #endregion
        #region To ClubStaffDto

        [TestMethod]
        public void ToClubStaffDto_FromClubStaffEntity_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = _clubStaffEntityBuilder.WithTrackingFields().Build();

            // Act
            ClubStaff result = clubStaffEntity.ToClubStaffDto();

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
        public void ToClubStaffDto_FromClubStaffEntity_ValidParameter_ShouldReturnClubStaffEntity()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = _clubStaffEntityBuilder.WithName().Build();

            // Act
            ClubStaff result = clubStaffEntity.ToClubStaffDto();

            // Assert
            Assert.AreEqual(clubStaffEntity.Id, result.Id);
            Assert.AreEqual(clubStaffEntity.FirstName, result.FirstName);
            Assert.AreEqual(clubStaffEntity.LastName, result.LastName);
            Assert.AreEqual(clubStaffEntity.Email, result.Email);
        }

        [TestMethod]
        public void ToClubStaffDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfClubStaffDto()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity1 = _clubStaffEntityBuilder.Build();
            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            ClubStaffEntity clubStaffEntity2 = _clubStaffEntityBuilder.Build();
            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            ClubStaffEntity clubStaffEntity3 = _clubStaffEntityBuilder.Build();
            List<ClubStaffEntity> clubStaffEntities = [clubStaffEntity1, clubStaffEntity2, clubStaffEntity3];

            // Act
            IEnumerable<ClubStaff> result = clubStaffEntities.ToClubStaffDtoEnumerable();

            // Assert
            Assert.AreEqual(clubStaffEntities.Count, result.Count());
            Assert.AreEqual(clubStaffEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubStaffEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubStaffEntities[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubStaffDtoEnumerable_NullClubStaff_ShouldThrowNullReferenceException()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity1 = _clubStaffEntityBuilder.Build();
            ClubStaffEntity clubStaffEntity2 = null!;
            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            ClubStaffEntity clubStaffEntity3 = _clubStaffEntityBuilder.Build();
            List<ClubStaffEntity> clubStaffs = [clubStaffEntity1, clubStaffEntity2, clubStaffEntity3];

            // Act & Assert
            var result = clubStaffs.ToClubStaffDtoEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
        }

        #endregion
        #region To ToAzureAdUser

        [TestMethod]
        public void ToAzureAdUser_FromClubStaffEntity_ValidParameter_ShouldReturnClubStaffEntity()
        {
            // Arrange
            ClubStaffEntity clubStaffEntity = _clubStaffEntityBuilder.WithName().Build();

            // Act
            var result = clubStaffEntity.ToAzureAdUser();

            // Assert
            Assert.AreEqual(clubStaffEntity.Id.ToString(), result.Id);
            Assert.AreEqual(clubStaffEntity.FirstName, result.GivenName);
            Assert.AreEqual(clubStaffEntity.LastName, result.Surname);
        }

        #endregion
    }
}
