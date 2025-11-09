using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Matches.Commands
{
    public record DeleteMatchCommand : IRequest
    {
        public int Id { get; init; }
    }

    public class DeleteMatchCommandHandler(AppDbContext db) : IRequestHandler<DeleteMatchCommand>
    {
        public async Task Handle(DeleteMatchCommand r, CancellationToken ct)
        {
            var matchToBeRemoved = await db.Matches.FirstOrDefaultAsync(x => x.Id == r.Id, ct);
            if (matchToBeRemoved == null)
            {
                throw new KeyNotFoundException("Match not found.");
            }

            db.Matches.Remove(matchToBeRemoved);
            await db.SaveChangesAsync(ct);
        }
    }
}
