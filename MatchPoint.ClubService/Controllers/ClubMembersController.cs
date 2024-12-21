using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MatchPoint.ClubService.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/clubs/{clubId:guid}/members")]
    [ApiController]
    public class ClubMembersController : ControllerBase
    {
    }
}
