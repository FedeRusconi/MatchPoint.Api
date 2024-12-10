using MatchPoint.Api.Shared.Common.Enums;

namespace MatchPoint.Api.Shared.Common.Interfaces
{
    public interface IDeactivable
    {
        public ActiveStatus ActiveStatus { get; set; }
    }
}
