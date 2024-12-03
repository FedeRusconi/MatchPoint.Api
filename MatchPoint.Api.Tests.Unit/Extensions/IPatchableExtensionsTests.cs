using MatchPoint.Api.Shared.Enums;
using MatchPoint.Api.Shared.Extensions;
using MatchPoint.Api.Shared.Models;
using MatchPoint.Api.Tests.Unit.Helpers;

namespace MatchPoint.Api.Tests.Unit.Extensions
{
    [TestClass]
    public class IPatchableExtensionsTests
    {
        [TestMethod]
        public void Patch_WhenValidTypesAndValuesAreProvided_ShouldUpdateProvidedFields()
        {
            #region Arrange
            var patchableEntity = new PatchableEntityTest();
            int intProperty = 1;
            int? nullableIntProperty = 2;
            double doubleProperty = 2.5;
            string stringProperty = "This is the edited value";
            string? nullableStringProperty = "This is another edited value";
            DateTime dateTimeProperty = new(2024, 12, 12, 10, 05, 12);
            bool boolProperty = true;
            ActiveStatus enumProperty = ActiveStatus.Active;
            var propertyUpdates = new List<PropertyUpdate>()
            {
                new(nameof(PatchableEntityTest.IntProperty), intProperty),
                new(nameof(PatchableEntityTest.NullableIntProperty), nullableIntProperty),
                new(nameof(PatchableEntityTest.DoubleProperty), doubleProperty),
                new(nameof(PatchableEntityTest.StringProperty), stringProperty),
                new(nameof(PatchableEntityTest.NullableStringProperty), nullableStringProperty),
                new(nameof(PatchableEntityTest.DateTimeProperty), dateTimeProperty),
                new(nameof(PatchableEntityTest.BoolProperty), boolProperty),
                new(nameof(PatchableEntityTest.EnumProperty), enumProperty),
            };
            #endregion

            #region Act
            patchableEntity.Patch(propertyUpdates);
            #endregion

            #region Assert
            Assert.AreEqual(intProperty, patchableEntity.IntProperty);
            Assert.AreEqual(nullableStringProperty, patchableEntity.NullableStringProperty);
            Assert.AreEqual(doubleProperty, patchableEntity.DoubleProperty);
            Assert.AreEqual(stringProperty, patchableEntity.StringProperty);
            Assert.AreEqual(nullableStringProperty, patchableEntity.NullableStringProperty);
            Assert.AreEqual(dateTimeProperty, patchableEntity.DateTimeProperty);
            Assert.AreEqual(boolProperty, patchableEntity.BoolProperty);
            Assert.AreEqual(enumProperty, patchableEntity.EnumProperty);
            #endregion
        }

        [TestMethod]
        public void Patch_WhenValidNullValuesAreProvided_ShouldUpdateProvidedFields()
        {
            #region Arrange
            var patchableEntity = new PatchableEntityTest()
            {
                NullableIntProperty = 1,
                NullableStringProperty = "Not Null"
            };
            var propertyUpdates = new List<PropertyUpdate>()
            {
                new(nameof(PatchableEntityTest.NullableIntProperty), null),
                new(nameof(PatchableEntityTest.NullableStringProperty), null)
            };
            #endregion

            #region Act
            patchableEntity.Patch(propertyUpdates);
            #endregion

            #region Assert
            Assert.IsNull(patchableEntity.NullableStringProperty);
            Assert.IsNull(patchableEntity.NullableStringProperty);
            #endregion
        }

        [TestMethod]
        [DataRow(nameof(PatchableEntityTest.IntProperty), null)]
        [DataRow(nameof(PatchableEntityTest.StringProperty), null)]
        public void Patch_WhenInvalidNullValuesAreProvided_ShouldThrowInvalidOperationException(
            string propertyName, string? value)
        {
            #region Arrange
            var patchableEntity = new PatchableEntityTest()
            {
                IntProperty = 1,
                StringProperty = "Not Nullable"
            };
            var propertyUpdates = new List<PropertyUpdate>()
            {
                new(propertyName, value)
            };
            #endregion

            #region Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => patchableEntity.Patch(propertyUpdates));
            #endregion
        }

        [TestMethod]
        public void Patch_WhenPropertyIsReadOnly_ShouldThrowInvalidOperationException()
        {
            #region Arrange
            var patchableEntity = new PatchableEntityTest();
            var propertyUpdates = new List<PropertyUpdate>()
            {
                new(nameof(PatchableEntityTest.ReadOnlyProperty), 3)
            };
            #endregion

            #region Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => patchableEntity.Patch(propertyUpdates));
            #endregion
        }

        [TestMethod]
        [DataRow(nameof(PatchableEntityTest.IntProperty), "Not an int")]
        [DataRow(nameof(PatchableEntityTest.DoubleProperty), "Not a double")]
        [DataRow(nameof(PatchableEntityTest.DateTimeProperty), "Not a datetime")]
        [DataRow(nameof(PatchableEntityTest.BoolProperty), "Not a bool")]
        [DataRow(nameof(PatchableEntityTest.EnumProperty), "Not an enum")]
        public void Patch_WhenInvalidTypesAndValuesAreProvided_ShouldThrowInvalidCastException(
            string propertyName, string value)
        {
            #region Arrange
            var patchableEntity = new PatchableEntityTest();
            var propertyUpdates = new List<PropertyUpdate>()
            {
                new(propertyName, value),
            };
            #endregion

            #region Act & Assert
            Assert.ThrowsException<InvalidCastException>(() => patchableEntity.Patch(propertyUpdates));
            #endregion
        }

        [TestMethod]
        [DataRow("PrivateProperty", 1)]
        [DataRow(nameof(PatchableEntityTest.StaticProperty), 1)]
        public void Patch_WhenPropertyIsPrivateOrStatic_ShouldThrowArgumentException(
            string propertyName, int value)
        {
            #region Arrange
            var patchableEntity = new PatchableEntityTest();
            var propertyUpdates = new List<PropertyUpdate>()
            {
                new(propertyName, value),
            };
            #endregion

            #region Act & Assert
            Assert.ThrowsException<ArgumentException>(() => patchableEntity.Patch(propertyUpdates));
            #endregion
        }

        [TestMethod]
        public void Patch_WhenListOfUpdatesIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            var patchableEntity = new PatchableEntityTest();
            List<PropertyUpdate> propertyUpdates = null!;
            #endregion

            #region Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => patchableEntity.Patch(propertyUpdates));
            #endregion
        }

        [TestMethod]
        public void Patch_WhenEntityIsNull_ShouldThrowArgumentNullException()
        {
            #region Arrange
            PatchableEntityTest patchableEntity = null!;
            List<PropertyUpdate> propertyUpdates = null!;
            #endregion

            #region Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => patchableEntity.Patch(propertyUpdates));
            #endregion
        }
    }
}
