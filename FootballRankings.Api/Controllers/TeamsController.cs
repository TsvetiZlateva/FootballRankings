using FootballRankings.Application.Common;
using FootballRankings.Application.Features.Teams;
using FootballRankings.Application.Features.Teams.Commands;
using FootballRankings.Application.Features.Teams.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FootballRankings.Api.Controllers
{
    public class TeamsController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TeamDto>), 200)]
        public async Task<IActionResult> Get([FromQuery] GetTeamsQuery query, CancellationToken ct)
        {
            var result = await Mediator.Send(query, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = nameof(GetTeamById))]
        [ProducesResponseType(typeof(TeamDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeamById(int id, CancellationToken ct)
        {
            var dto = await Mediator.Send(new GetTeamByIdQuery { Id = id }, ct);
            return Ok(dto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TeamDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateTeamCommand command, CancellationToken ct)
        {
            var dto = await Mediator.Send(command, ct);
            return CreatedAtRoute(nameof(GetTeamById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TeamDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamCommand command, CancellationToken ct)
        {
            command.Id = id;
            var dto = await Mediator.Send(command, ct);
            return Ok(dto);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await Mediator.Send(new DeleteTeamCommand { Id = id }, ct);
            return NoContent();
        }
    }
}
