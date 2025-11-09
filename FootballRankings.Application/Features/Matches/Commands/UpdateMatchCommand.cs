using FluentValidation;
using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Matches.Commands
{
    public record UpdateMatchCommand : IRequest<MatchDto>
    {
        public int Id { get; set; }
        public int HomeGoals { get; init; }
        public int AwayGoals { get; init; }
        public DateTime PlayedAt { get; init; }
        public string RowVersion { get; init; } = "";
    }

    public class UpdateMatchCommandValidator : AbstractValidator<UpdateMatchCommand>
    {
        public UpdateMatchCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.HomeGoals).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AwayGoals).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RowVersion).NotEmpty();
            RuleFor(x => x.PlayedAt).LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1));
        }
    }

    public class UpdateMatchCommandHandler(AppDbContext db) : IRequestHandler<UpdateMatchCommand, MatchDto>
    {
        public async Task<MatchDto> Handle(UpdateMatchCommand r, CancellationToken ct)
        {
            var matchToBeUpdated = await db.Matches
                .Include(x => x.HomeTeam)
                .Include(x => x.AwayTeam)
                .FirstOrDefaultAsync(x => x.Id == r.Id, ct);

            if (matchToBeUpdated == null)
            {
                throw new KeyNotFoundException("Match not found.");
            }

            db.Entry(matchToBeUpdated).Property(x => x.RowVersion).OriginalValue = Convert.FromBase64String(r.RowVersion);

            matchToBeUpdated.HomeGoals = r.HomeGoals;
            matchToBeUpdated.AwayGoals = r.AwayGoals;
            matchToBeUpdated.PlayedAt = r.PlayedAt.ToUniversalTime();

            await db.SaveChangesAsync(ct);

            return new MatchDto
            {
                Id = matchToBeUpdated.Id,
                HomeTeamId = matchToBeUpdated.HomeTeamId,
                HomeTeamName = matchToBeUpdated.HomeTeam.Name,
                AwayTeamId = matchToBeUpdated.AwayTeamId,
                AwayTeamName = matchToBeUpdated.AwayTeam.Name,
                HomeGoals = matchToBeUpdated.HomeGoals,
                AwayGoals = matchToBeUpdated.AwayGoals,
                PlayedAt = matchToBeUpdated.PlayedAt,
                RowVersion = Convert.ToBase64String(matchToBeUpdated.RowVersion)
            };
        }
    }
}
