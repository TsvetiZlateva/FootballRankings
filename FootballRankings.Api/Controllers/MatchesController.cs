using FootballRankings.Application.Common;
using FootballRankings.Application.Features.Matches;
using FootballRankings.Application.Features.Matches.Commands;
using FootballRankings.Application.Features.Matches.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FootballRankings.Api.Controllers
{
    public class MatchesController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<MatchDto>), 200)]
        public async Task<IActionResult> Get([FromQuery] GetMatchesQuery query, CancellationToken ct)
        {
            var result = await Mediator.Send(query, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(MatchDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var dto = await Mediator.Send(new GetMatchByIdQuery { Id = id }, ct);
            return Ok(dto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MatchDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateMatchCommand command, CancellationToken ct)
        {
            var dto = await Mediator.Send(command, ct);
            return CreatedAtRoute(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(MatchDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(
            int id, 
            [FromBody] UpdateMatchCommand command, 
            CancellationToken ct)
        {
            command.Id = id;
            var dto = await Mediator.Send(command, ct);
            return Ok(dto);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await Mediator.Send(new DeleteMatchCommand { Id = id }, ct);
            return NoContent();
        }
    }
}
