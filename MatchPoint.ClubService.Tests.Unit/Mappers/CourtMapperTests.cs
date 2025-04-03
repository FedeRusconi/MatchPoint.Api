using System.Reflection;
using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.CourtService.Models;
using MatchPoint.ClubService.Entities;
using MatchPoint.ClubService.Mappers;

namespace MatchPoint.ClubService.Tests.Unit.Mappers
{
    [TestClass]
    public class CourtMapperTests
    {
        // This is used to ensure no properties are forgotten
        private readonly string[] expectedCourtEntityProperties = [
            nameof(ClubCourtEntity.Id), nameof(ClubCourtEntity.Name), nameof(ClubCourtEntity.ActiveStatus)];
        private readonly string[] expectedCourtDtoProperties = [
            nameof(Court.Id), nameof(Court.Name), nameof(Court.ActiveStatus)];

        #region To CourtEntity

        [TestMethod]
        public void ToCourtEntity_FromCourtDto_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            Court courtDto = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1",
                ActiveStatus = ActiveStatus.Inactive
            };

            // Act
            ClubCourtEntity result = courtDto.ToCourtEntity();

            // Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(properties.All(prop => expectedCourtEntityProperties.Contains(prop.Name)));
        }

        [TestMethod]
        public void ToCourtEntity_FromCourtDto_ValidParameter_ShouldReturnCourtEntity()
        {
            // Arrange
            Court courtDto = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1",
                ActiveStatus = ActiveStatus.Inactive
            };

            // Act
            ClubCourtEntity result = courtDto.ToCourtEntity();

            // Assert
            Assert.AreEqual(courtDto.Id, result.Id);
            Assert.AreEqual(courtDto.Name, result.Name);
            Assert.AreEqual(courtDto.ActiveStatus, result.ActiveStatus);
        }

        [TestMethod]
        public void ToCourtEntityEnumerable_ValidParameter_ShouldReturnEnumerableOfCourtEntity()
        {
            // Arrange
            Court court1 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1"
            };
            Court court2 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 2"
            };
            Court court3 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 3",
            };
            List<Court> courts = [court1, court2, court3];

            // Act
            IEnumerable<ClubCourtEntity> result = courts.ToCourtEntityEnumerable();

            // Assert
            Assert.AreEqual(courts.Count, result.Count());
            Assert.AreEqual(courts[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(courts[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(courts[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToCourtEntityEnumerable_NullCourt_ShouldThrowNullReferenceException()
        {
            //  Arrange
            Court court1 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1"
            };
            Court court2 = null!;
            Court court3 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 3",
            };
            List<Court> courts = [court1, court2, court3];

            // Act & Assert
            var result = courts.ToCourtEntityEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
        #region To CourtDto

        [TestMethod]
        public void ToCourtDto_FromCourtEntity_AllExpectedPropertiesShouldBeSet()
        {
            // Arrange
            ClubCourtEntity courtEntity = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1",
                ActiveStatus = ActiveStatus.Inactive
            };

            // Act
            Court result = courtEntity.ToCourtDto();

            // Assert
            PropertyInfo[] properties = result.GetType().GetProperties();
            Assert.IsTrue(properties.All(prop => expectedCourtDtoProperties.Contains(prop.Name)));
        }

        [TestMethod]
        public void ToCourtDto_FromCourtEntity_ValidParameter_ShouldReturnCourtDto()
        {
            // Arrange
            ClubCourtEntity courtEntity = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1",
                ActiveStatus = ActiveStatus.Inactive
            };

            // Act
            Court result = courtEntity.ToCourtDto();

            // Assert
            Assert.AreEqual(courtEntity.Id, result.Id);
            Assert.AreEqual(courtEntity.Name, result.Name);
            Assert.AreEqual(courtEntity.ActiveStatus, result.ActiveStatus);
        }

        [TestMethod]
        public void ToCourtDtoEnumerable_ValidParameter_ShouldReturnEnumerableOfCourtDto()
        {
            // Arrange
            ClubCourtEntity courtEntity1 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1"
            };
            ClubCourtEntity courtEntity2 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 2"
            };
            ClubCourtEntity courtEntity3 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 3",
            };
            List<ClubCourtEntity> courtEntities = [courtEntity1, courtEntity2, courtEntity3];

            // Act
            IEnumerable<Court> result = courtEntities.ToCourtDtoEnumerable();

            // Assert
            Assert.AreEqual(courtEntities.Count, result.Count());
            Assert.AreEqual(courtEntities[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(courtEntities[1].Id, result.ElementAt(1).Id);
            Assert.AreEqual(courtEntities[2].Id, result.ElementAt(2).Id);
        }

        [TestMethod]
        public void ToCourtDtoEnumerable_NullCourt_ShouldThrowNullReferenceException()
        {
            // Arrange
            ClubCourtEntity courtEntity1 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 1"
            };
            ClubCourtEntity courtEntity2 = null!;
            ClubCourtEntity courtEntity3 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Court 3",
            };
            List<ClubCourtEntity> courtEntities = [courtEntity1, courtEntity2, courtEntity3];

            // Act & Assert
            var result = courtEntities.ToCourtDtoEnumerable();
            Assert.ThrowsExactly<NullReferenceException>(() => _ = result.ToList());
        }

        #endregion
    }
}
