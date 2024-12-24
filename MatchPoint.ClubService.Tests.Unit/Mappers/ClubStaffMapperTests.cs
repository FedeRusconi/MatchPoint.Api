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

        // This is used to ensure no properties are forgotten
        private readonly string[] expectedClubStaffEntityProperties = [
            nameof(ClubStaffEntity.Id), nameof(ClubStaffEntity.Email), nameof(ClubStaffEntity.FirstName), 
            nameof(ClubStaffEntity.LastName), nameof(ClubStaffEntity.Photo)];
        private readonly string[] expectedClubStaffDtoProperties = [
            nameof(ClubStaff.Id), nameof(ClubStaff.Email), nameof(ClubStaff.FirstName), 
            nameof(ClubStaff.LastName), nameof(ClubStaff.Photo)];

        [TestInitialize]
        public void Setup()
        {
            _clubStaffEntityBuilder = new();
        }

        #region To ClubStaffEntity

        [TestMethod]
        public void ToClubStaffEntity_FromClubStaffDto_AllExpectedPropertiesShouldBeSet()
        {
            #region Arrange
            ClubStaff clubStaffDto = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL"
            };
            #endregion

            #region Act
            ClubStaffEntity result = clubStaffDto.ToClubStaffEntity();
            #endregion

            #region Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(expectedClubStaffEntityProperties.All(prop => properties.Select(p => p.Name).Contains(prop)));
            #endregion
        }

        [TestMethod]
        public void ToClubStaffEntity_FromClubStaffDto_ValidParameter_ShouldReturnClubStaffEntity()
        {
            #region Arrange
            ClubStaff clubStaffDto = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                FirstName = "First",
                LastName = "Last",
                Photo = "PhotoURL",
                RoleId = Guid.NewGuid(),
                RoleName = "Role Test",
                ClubId = Guid.NewGuid(),
            };
            #endregion

            #region Act
            ClubStaffEntity result = clubStaffDto.ToClubStaffEntity();
            #endregion

            #region Assert
            Assert.AreEqual(clubStaffDto.Id, result.Id);
            Assert.AreEqual(clubStaffDto.Email, result.Email);
            Assert.AreEqual(clubStaffDto.Photo, result.Photo);
            Assert.AreEqual(clubStaffDto.RoleId, result.RoleId);
            Assert.AreEqual(clubStaffDto.RoleName, result.RoleName);
            Assert.AreEqual(clubStaffDto.ClubId, result.ClubId);
            #endregion
        }

        [TestMethod]
        public void ToClubStaffEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfClubStaffEntity()
        {
            #region Arrange
            ClubStaff clubStaff1 = new()
            {
                Id = Guid.NewGuid(),
                Email = "test1@test.com",
                FirstName = "First 1",
                LastName = "Last 1"
            };
            ClubStaff clubStaff2 = new()
            {
                Id = Guid.NewGuid(),
                Email = "test2@test.com",
                FirstName = "First 2",
                LastName = "Last 2"
            };
            ClubStaff clubStaff3 = new()
            {
                Id = Guid.NewGuid(),
                Email = "test3@test.com",
                FirstName = "First 3",
                LastName = "Last 3"
            };
            List<ClubStaff> clubStaffs = [clubStaff1, clubStaff2, clubStaff3];
            #endregion

            #region  Act
            IEnumerable<ClubStaffEntity> result = clubStaffs.ToClubStaffEntityEnumerable();
            #endregion

            #region Assert
            Assert.AreEqual(clubStaffs.Count, result.Count());
            Assert.AreEqual(clubStaffs[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubStaffs[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubStaffs[2].Id, result.ElementAt(2).Id);
            #endregion
        }

        [TestMethod]
        public void ToClubStaffEntityEnumerable_NullClubStaff_ShouldThrowNullReferenceException()
        {
            #region  Arrange
            ClubStaff clubStaff1 = new()
            {
                Id = Guid.NewGuid(),
                Email = "test1@test.com",
                FirstName = "First 1",
                LastName = "Last 1"
            };
            ClubStaff clubStaff2 = null!;
            ClubStaff clubStaff3 = new()
            {
                Id = Guid.NewGuid(),
                Email = "test3@test.com",
                FirstName = "First 3",
                LastName = "Last 3"
            };
            List<ClubStaff> clubStaffs = [clubStaff1, clubStaff2, clubStaff3];
            #endregion

            #region Act & Assert
            var result = clubStaffs.ToClubStaffEntityEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
            #endregion
        }

        #endregion
        #region To ClubStaffDto

        [TestMethod]
        public void ToClubStaffDto_FromClubStaffEntity_AllExpectedPropertiesShouldBeSet()
        {
            #region Arrange
            ClubStaffEntity clubStaffEntity = _clubStaffEntityBuilder.Build();
            #endregion

            #region Act
            ClubStaff result = clubStaffEntity.ToClubStaffDto();
            #endregion

            #region Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(expectedClubStaffDtoProperties.All(prop => properties.Select(p => p.Name).Contains(prop)));
            #endregion
        }

        [TestMethod]
        public void ToClubStaffDto_FromClubStaffEntity_ValidParameter_ShouldReturnClubStaffEntity()
        {
            #region Arrange
            ClubStaffEntity clubStaffEntity = _clubStaffEntityBuilder.WithName().Build();
            #endregion

            #region Act
            ClubStaff result = clubStaffEntity.ToClubStaffDto();
            #endregion

            #region Assert
            Assert.AreEqual(clubStaffEntity.Id, result.Id);
            Assert.AreEqual(clubStaffEntity.FirstName, result.FirstName);
            Assert.AreEqual(clubStaffEntity.LastName, result.LastName);
            Assert.AreEqual(clubStaffEntity.Email, result.Email);
            #endregion
        }

        [TestMethod]
        public void ToClubStaffDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfClubStaffDto()
        {
            #region Arrange
            ClubStaffEntity clubStaffEntity1 = _clubStaffEntityBuilder.Build();
            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            ClubStaffEntity clubStaffEntity2 = _clubStaffEntityBuilder.Build();
            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            ClubStaffEntity clubStaffEntity3 = _clubStaffEntityBuilder.Build();
            List<ClubStaffEntity> clubStaffEntities = [clubStaffEntity1, clubStaffEntity2, clubStaffEntity3];
            #endregion

            #region  Act
            IEnumerable<ClubStaff> result = clubStaffEntities.ToClubStaffDtoEnumerable();
            #endregion

            #region Assert
            Assert.AreEqual(clubStaffEntities.Count, result.Count());
            Assert.AreEqual(clubStaffEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubStaffEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubStaffEntities[2].Id, result.ElementAt(2).Id);
            #endregion
        }

        [TestMethod]
        public void ToClubStaffDtoEnumerable_NullClubStaff_ShouldThrowNullReferenceException()
        {
            #region  Arrange
            ClubStaffEntity clubStaffEntity1 = _clubStaffEntityBuilder.Build();
            ClubStaffEntity clubStaffEntity2 = null!;
            _clubStaffEntityBuilder = new ClubStaffEntityBuilder();
            ClubStaffEntity clubStaffEntity3 = _clubStaffEntityBuilder.Build();
            List<ClubStaffEntity> clubStaffs = [clubStaffEntity1, clubStaffEntity2, clubStaffEntity3];
            #endregion

            #region Act & Assert
            var result = clubStaffs.ToClubStaffDtoEnumerable();
            Assert.ThrowsException<NullReferenceException>(result.ToList);
            #endregion
        }

        #endregion
    }
}
