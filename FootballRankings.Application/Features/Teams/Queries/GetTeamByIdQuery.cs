using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Teams.Queries
{
    public record GetTeamByIdQuery : IRequest<TeamDto>
    {
        public int Id { get; init; }
    }

    public class GetTeamByIdQueryHandler(AppDbContext db) : IRequestHandler<GetTeamByIdQuery, TeamDto>
    {
        public async Task<TeamDto> Handle(GetTeamByIdQuery r, CancellationToken ct)
        {
            var team = await db.Teams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == r.Id, ct);

            if (team == null)
            {
                throw new KeyNotFoundException("Team not found.");
            }

            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                FoundedYear = team.FoundedYear,
                Coach = team.Coach,
                RowVersion = Convert.ToBase64String(team.RowVersion)
            };
        }
    }
}
