using System.Diagnostics.CodeAnalysis;

namespace MatchPoint.Api.Shared.Common.Models
{
    public class PropertyUpdate
    {
        public required string Property { get; set; }
        public object? Value { get; set; }

        public PropertyUpdate() { }

        [SetsRequiredMembers]
        public PropertyUpdate(string property, object? value)
        {
            Property = property;
            Value = value;
        }
    }
}
