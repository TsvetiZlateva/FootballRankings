using FootballRankings.Application.Common;
using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Teams.Queries
{
    public record GetTeamsQuery : IRequest<PagedResult<TeamDto>>
    {
        public string? Search { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }

    public class GetTeamsQueryHandler(AppDbContext db) : IRequestHandler<GetTeamsQuery, PagedResult<TeamDto>>
    {
        public async Task<PagedResult<TeamDto>> Handle(GetTeamsQuery r, CancellationToken ct)
        {
            var teamsQuery = db.Teams.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(r.Search))
            {
                var term = r.Search.Trim();
                teamsQuery = teamsQuery.Where(t => t.Name.Contains(term));
            }

            var total = await teamsQuery.CountAsync(ct);

            var items = await teamsQuery.OrderBy(t => t.Name)
                .Skip((r.Page - 1) * r.PageSize)
                .Take(r.PageSize)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    FoundedYear = t.FoundedYear,
                    Coach = t.Coach
                })
                .ToListAsync(ct);

            return new PagedResult<TeamDto>(items, total, r.Page, r.PageSize);
        }
    }
}
