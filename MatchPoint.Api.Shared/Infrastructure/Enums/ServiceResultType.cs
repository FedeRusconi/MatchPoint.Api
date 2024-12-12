namespace MatchPoint.Api.Shared.Infrastructure.Enums
{
    public enum ServiceResultType
    {
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        InternalError = 500
    }
}
