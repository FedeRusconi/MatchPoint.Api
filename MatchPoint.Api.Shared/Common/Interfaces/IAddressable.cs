using MatchPoint.Api.Shared.Common.Models;

namespace MatchPoint.Api.Shared.Common.Interfaces
{
    public interface IAddressable
    {
        public Address Address { get; set; }
    }
}
