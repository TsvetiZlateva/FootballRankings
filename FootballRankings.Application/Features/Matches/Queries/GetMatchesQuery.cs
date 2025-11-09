using FootballRankings.Application.Common;
using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Matches.Queries
{
    public record GetMatchesQuery : IRequest<PagedResult<MatchDto>>
    {
        public int? TeamId { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }

    public class GetMatchesQueryHandler(AppDbContext db) : IRequestHandler<GetMatchesQuery, PagedResult<MatchDto>>
    {
        public async Task<PagedResult<MatchDto>> Handle(GetMatchesQuery r, CancellationToken ct)
        {
            var matchesQuery = db.Matches.AsNoTracking()
                .Include(x => x.HomeTeam)
                .Include(x => x.AwayTeam)
                .AsQueryable();

            if (r.TeamId.HasValue)
            {
                matchesQuery = matchesQuery.Where(x => x.HomeTeamId == r.TeamId.Value || x.AwayTeamId == r.TeamId.Value);
            }
            if (r.From.HasValue)
            {
                matchesQuery = matchesQuery.Where(x => x.PlayedAt >= r.From.Value);
            }
            if (r.To.HasValue)
            {
                matchesQuery = matchesQuery.Where(x => x.PlayedAt <= r.To.Value);
            }

            var total = await matchesQuery.CountAsync(ct);

            var items = await matchesQuery.OrderByDescending(x => x.PlayedAt)
                .Skip((r.Page - 1) * r.PageSize)
                .Take(r.PageSize)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam.Name,
                    HomeGoals = m.HomeGoals,
                    AwayGoals = m.AwayGoals,
                    PlayedAt = m.PlayedAt
                })
                .ToListAsync(ct);

            return new PagedResult<MatchDto>(items, total, r.Page, r.PageSize);
        }
    }
}
