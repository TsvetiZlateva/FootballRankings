using FootballRankings.Application.Features.Standings;
using FootballRankings.Application.Features.Standings.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FootballRankings.Api.Controllers
{
    public class StandingsController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<StandingDto>), 200)]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var list = await Mediator.Send(new GetStandingsQuery(), ct);
            return Ok(list);
        }
    }
}
