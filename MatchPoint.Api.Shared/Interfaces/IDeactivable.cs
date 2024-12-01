using MatchPoint.Api.Shared.Enums;

namespace MatchPoint.Api.Shared.Interfaces
{
    public interface IDeactivable
    {
        public ActiveStatus ActiveStatus { get; set; }
    }
}
