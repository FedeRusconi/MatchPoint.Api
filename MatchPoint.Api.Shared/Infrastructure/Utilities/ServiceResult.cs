using MatchPoint.Api.Shared.Infrastructure.Enums;
using MatchPoint.Api.Shared.Infrastructure.Interfaces;

namespace MatchPoint.Api.Shared.Infrastructure.Utilities
{
    public class ServiceResult<T> : IServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public ServiceResultType ResultType { get; set; }

        public static ServiceResult<T> Success(T data) =>
            new() { IsSuccess = true, Data = data, ResultType = ServiceResultType.Success };

        public static ServiceResult<T> Failure(string error, ServiceResultType resultType) =>
            new() { IsSuccess = false, Error = error, ResultType = resultType };
    }
}
