using MatchPoint.Api.Shared.Infrastructure.Enums;

namespace MatchPoint.Api.Shared.Infrastructure.Interfaces
{
    public interface IServiceResult<T>
    {
        public T? Data { get; set; }
        public string? Error { get; set; }
        public bool IsSuccess { get; set; }
        public ServiceResultType ResultType { get; set; }
    }
}