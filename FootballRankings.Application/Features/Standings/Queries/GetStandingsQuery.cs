using FootballRankings.Application.Common.Interfaces;
using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Standings.Queries
{
    public record GetStandingsQuery : IRequest<IReadOnlyList<StandingDto>>;

    public class GetStandingsQueryHandler(AppDbContext db, IScoringStrategy scoring)
        : IRequestHandler<GetStandingsQuery, IReadOnlyList<StandingDto>>
    {
        public async Task<IReadOnlyList<StandingDto>> Handle(GetStandingsQuery r, CancellationToken ct)
        {
            var home = db.Matches.Select(m => new
            {
                TeamId = m.HomeTeamId,
                GoalsFor = m.HomeGoals,
                GoalsAgainst = m.AwayGoals,
                Played = 1,
                Won = m.HomeGoals > m.AwayGoals ? 1 : 0,
                Drawn = m.HomeGoals == m.AwayGoals ? 1 : 0,
                Lost = m.HomeGoals < m.AwayGoals ? 1 : 0,
                Points = m.HomeGoals > m.AwayGoals ? 3 : (m.HomeGoals == m.AwayGoals ? 1 : 0)
            });

            var away = db.Matches.Select(m => new
            {
                TeamId = m.AwayTeamId,
                GoalsFor = m.AwayGoals,
                GoalsAgainst = m.HomeGoals,
                Played = 1,
                Won = m.AwayGoals > m.HomeGoals ? 1 : 0,
                Drawn = m.AwayGoals == m.HomeGoals ? 1 : 0,
                Lost = m.AwayGoals < m.HomeGoals ? 1 : 0,
                Points = m.AwayGoals > m.HomeGoals ? 3 : (m.AwayGoals == m.HomeGoals ? 1 : 0)
            });

            var participations = home.Concat(away);

            var standingsQuery = db.Teams
                .AsNoTracking()
                .GroupJoin(
                    participations,
                    t => t.Id,
                    p => p.TeamId,
                    (t, gp) => new StandingDto
                    {
                        TeamId = t.Id,
                        TeamName = t.Name,
                        Played = gp.Sum(x => (int?)x.Played) ?? 0,
                        Won = gp.Sum(x => (int?)x.Won) ?? 0,
                        Drawn = gp.Sum(x => (int?)x.Drawn) ?? 0,
                        Lost = gp.Sum(x => (int?)x.Lost) ?? 0,
                        GoalsFor = gp.Sum(x => (int?)x.GoalsFor) ?? 0,
                        GoalsAgainst = gp.Sum(x => (int?)x.GoalsAgainst) ?? 0,
                        GoalDifference = (gp.Sum(x => (int?)x.GoalsFor) ?? 0) - (gp.Sum(x => (int?)x.GoalsAgainst) ?? 0),
                        Points = gp.Sum(x => (int?)x.Points) ?? 0
                    });

            var ordered =
                await standingsQuery
                    .OrderByDescending(s => s.Points)
                    .ThenByDescending(s => s.GoalDifference)
                    .ThenByDescending(s => s.GoalsFor)
                    .ThenBy(s => s.TeamName)
                    .ToListAsync(ct);

            return ordered;
        }

    }
}
