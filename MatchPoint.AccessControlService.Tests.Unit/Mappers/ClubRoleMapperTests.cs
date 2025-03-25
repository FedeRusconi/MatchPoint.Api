using System.Reflection;
using MatchPoint.AccessControlService.Entities;
using MatchPoint.AccessControlService.Mappers;
using MatchPoint.Api.Shared.AccessControlService.Models;
using MatchPoint.Api.Tests.Shared.AccessControlService.Helpers;

namespace MatchPoint.AccessControlService.Tests.Unit.Mappers
{
    [TestClass]
    public class ClubRoleMapperTests
    {
        private ClubRoleEntityBuilder _clubRoleEntityBuilder = default!;
        private ClubRoleBuilder _clubRoleBuilder = default!;

        [TestInitialize]
        public void Setup()
        {
            _clubRoleEntityBuilder = new();
            _clubRoleBuilder = new();
        }

        #region ToClubRoleEntity

        [TestMethod]
        public void ToClubRoleEntity_FromClubRoleDto_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubRole clubRoleDto = _clubRoleBuilder.WithTrackingFields().Build();

            // Act
            ClubRoleEntity result = clubRoleDto.ToClubRoleEntity();

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
        public void ToClubRoleEntity_FromClubRoleDto_ValidParameter_ShouldReturnClubRoleEntity()
        {
            // Arrange
            ClubRole clubRoleDto = _clubRoleBuilder.WithTrackingFields().Build();

            // Act
            ClubRoleEntity result = clubRoleDto.ToClubRoleEntity();

            // Assert
            Assert.AreEqual(clubRoleDto.Id, result.Id);
            Assert.AreEqual(clubRoleDto.ClubId, result.ClubId);
            Assert.AreEqual(clubRoleDto.Name, result.Name);
            Assert.IsTrue(result.Capabilities.All(clubRoleDto.Capabilities.Contains));
            Assert.AreEqual(clubRoleDto.ActiveStatus, result.ActiveStatus);
            Assert.AreEqual(clubRoleDto.CreatedBy, result.CreatedBy);
            Assert.AreEqual(clubRoleDto.CreatedOnUtc, result.CreatedOnUtc);
            Assert.AreEqual(clubRoleDto.ModifiedBy, result.ModifiedBy);
            Assert.AreEqual(clubRoleDto.ModifiedOnUtc, result.ModifiedOnUtc);
        }

        [TestMethod]
        public void ToClubRoleEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfClubRoleEntity()
        {
            // Arrange
            ClubRole clubRole1 = _clubRoleBuilder.WithTrackingFields().Build();
            _clubRoleBuilder = new();
            ClubRole clubRole2 = _clubRoleBuilder.WithTrackingFields().Build();
            _clubRoleBuilder = new();
            ClubRole clubRole3 = _clubRoleBuilder.WithTrackingFields().Build();
            List<ClubRole> clubRoles = [clubRole1, clubRole2, clubRole3];

            // Act
            IEnumerable<ClubRoleEntity> result = clubRoles.ToClubRoleEntityEnumerable();

            // Assert
            Assert.AreEqual(clubRoles.Count, result.Count());
            Assert.AreEqual(clubRoles[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubRoles[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubRoles[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubRoleEntityEnumerable_NullClubRole_ShouldThrowNullReferenceException()
        {
            // Arrange
            ClubRole clubRole1 = _clubRoleBuilder.WithTrackingFields().Build();
            ClubRole clubRole2 = null!;
            _clubRoleBuilder = new();
            ClubRole clubRole3 = _clubRoleBuilder.WithTrackingFields().Build();
            List<ClubRole> clubRoles = [clubRole1, clubRole2, clubRole3];

            // Act & Assert
            var result = clubRoles.ToClubRoleEntityEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
        #region ToClubRoleDto

        [TestMethod]
        public void ToClubRoleDto_FromClubRoleEntity_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = _clubRoleEntityBuilder.WithTrackingFields().Build();

            // Act
            ClubRole result = clubRoleEntity.ToClubRoleDto();

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
        public void ToClubRoleDto_FromClubRoleEntity_ValidParameter_ShouldReturnClubRoleEntity()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity = _clubRoleEntityBuilder.WithTrackingFields().Build();

            // Act
            ClubRole result = clubRoleEntity.ToClubRoleDto();

            // Assert
            Assert.AreEqual(clubRoleEntity.Id, result.Id);
            Assert.AreEqual(clubRoleEntity.ClubId, result.ClubId);
            Assert.AreEqual(clubRoleEntity.Name, result.Name);
            Assert.IsTrue(result.Capabilities.All(clubRoleEntity.Capabilities.Contains));
            Assert.AreEqual(clubRoleEntity.ActiveStatus, result.ActiveStatus);
            Assert.AreEqual(clubRoleEntity.CreatedBy, result.CreatedBy);
            Assert.AreEqual(clubRoleEntity.CreatedOnUtc, result.CreatedOnUtc);
            Assert.AreEqual(clubRoleEntity.ModifiedBy, result.ModifiedBy);
            Assert.AreEqual(clubRoleEntity.ModifiedOnUtc, result.ModifiedOnUtc);
        }

        [TestMethod]
        public void ToClubRoleDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfClubRoleDto()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity1 = _clubRoleEntityBuilder.Build();
            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            ClubRoleEntity clubRoleEntity2 = _clubRoleEntityBuilder.Build();
            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            ClubRoleEntity clubRoleEntity3 = _clubRoleEntityBuilder.Build();
            List<ClubRoleEntity> clubRoleEntities = [clubRoleEntity1, clubRoleEntity2, clubRoleEntity3];

            // Act
            IEnumerable<ClubRole> result = clubRoleEntities.ToClubRoleDtoEnumerable();

            // Assert
            Assert.AreEqual(clubRoleEntities.Count, result.Count());
            Assert.AreEqual(clubRoleEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(clubRoleEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(clubRoleEntities[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToClubRoleDtoEnumerable_NullClubRoleEntity_ShouldThrowNullReferenceException()
        {
            // Arrange
            ClubRoleEntity clubRoleEntity1 = _clubRoleEntityBuilder.Build();
            ClubRoleEntity clubRoleEntity2 = null!;
            _clubRoleEntityBuilder = new ClubRoleEntityBuilder();
            ClubRoleEntity clubRoleEntity3 = _clubRoleEntityBuilder.Build();
            List<ClubRoleEntity> clubRoles = [clubRoleEntity1, clubRoleEntity2, clubRoleEntity3];

            // Act & Assert
            var result = clubRoles.ToClubRoleDtoEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
    }
}
