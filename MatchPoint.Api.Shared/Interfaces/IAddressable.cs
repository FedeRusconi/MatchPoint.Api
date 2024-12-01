using MatchPoint.Api.Shared.Models;

namespace MatchPoint.Api.Shared.Interfaces
{
    public interface IAddressable
    {
        public Address Address { get; set; }
    }
}
