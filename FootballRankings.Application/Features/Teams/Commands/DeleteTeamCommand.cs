using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Teams.Commands
{
    public record DeleteTeamCommand : IRequest
    {
        public int Id { get; init; }
    }

    public class DeleteTeamCommandHandler(AppDbContext db) : IRequestHandler<DeleteTeamCommand>
    {
        public async Task Handle(DeleteTeamCommand r, CancellationToken ct)
        {
            var hasMatches = await db.Matches.AnyAsync(m => m.HomeTeamId == r.Id || m.AwayTeamId == r.Id, ct);
            if (hasMatches)
            {
                throw new InvalidOperationException("Cannot delete a team that has played matches.");
            }

            var teamToBeDeleted = await db.Teams.FirstOrDefaultAsync(t => t.Id == r.Id, ct);

            if (teamToBeDeleted == null)
            {
                throw new KeyNotFoundException("Team not found.");
            }

            db.Teams.Remove(teamToBeDeleted);
            await db.SaveChangesAsync(ct);
        }
    }
}
