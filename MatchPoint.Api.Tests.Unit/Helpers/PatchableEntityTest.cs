using MatchPoint.Api.Shared.Common.Enums;
using MatchPoint.Api.Shared.Common.Interfaces;

namespace MatchPoint.Api.Tests.Unit.Helpers
{
    internal class PatchableEntityTest : IPatchable
    {
        public int IntProperty { get; set; }        
        public double DoubleProperty { get; set; }
        public string StringProperty { get; set; } = string.Empty;        
        public DateTime DateTimeProperty { get; set; }
        public bool BoolProperty { get; set; }
        public ActiveStatus EnumProperty { get; set; }

        public int? NullableIntProperty { get; set; }
        public double? NullableDoubleProperty { get; set; }
        public string? NullableStringProperty { get; set; }
        public DateTime? NullableDateTimeProperty { get; set; }
        public bool? NullableBoolProperty { get; set; }
        public ActiveStatus? NullableEnumProperty { get; set; }

        public int ReadOnlyProperty => 0;
        private int PrivateProperty {  get; set; }
        public static int StaticProperty {  get; set; }
    }
}
