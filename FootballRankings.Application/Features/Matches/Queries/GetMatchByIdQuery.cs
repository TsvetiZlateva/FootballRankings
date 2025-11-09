using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Matches.Queries
{
    public record GetMatchByIdQuery : IRequest<MatchDto>
    {
        public int Id { get; init; }
    }

    public class GetMatchByIdQueryHandler(AppDbContext db) : IRequestHandler<GetMatchByIdQuery, MatchDto>
    {
        public async Task<MatchDto> Handle(GetMatchByIdQuery r, CancellationToken ct)
        {
            var match = await db.Matches.AsNoTracking()
                        .Include(x => x.HomeTeam)
                        .Include(x => x.AwayTeam)
                        .FirstOrDefaultAsync(x => x.Id == r.Id, ct);
            if (match == null)
            {
                throw new KeyNotFoundException("Match not found.");
            }

            return new MatchDto
            {
                Id = match.Id,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeam.Name,
                HomeGoals = match.HomeGoals,
                AwayGoals = match.AwayGoals,
                PlayedAt = match.PlayedAt
            };
        }
    }
}
