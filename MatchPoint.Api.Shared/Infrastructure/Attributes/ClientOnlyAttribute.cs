namespace MatchPoint.Api.Shared.Infrastructure.Attributes
{
    /// <summary>
    /// This attribute is only used as a marker for client-only properties of a class.
    /// By applying this, automated tests will ignore the given property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ClientOnlyAttribute : Attribute
    {
    }
}
