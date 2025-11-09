using FluentValidation;
using FootballRankings.Data.Data;
using FootballRankings.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Matches.Commands
{
    public record CreateMatchCommand : IRequest<MatchDto>
    {
        public int HomeTeamId { get; init; }
        public int AwayTeamId { get; init; }
        public int HomeGoals { get; init; }
        public int AwayGoals { get; init; }
        public DateTime PlayedAt { get; init; } 
    }

    public class CreateMatchCommandValidator : AbstractValidator<CreateMatchCommand>
    {
        public CreateMatchCommandValidator()
        {
            RuleFor(x => x.HomeTeamId).GreaterThan(0);
            RuleFor(x => x.AwayTeamId).GreaterThan(0);
            RuleFor(x => x.AwayTeamId).NotEqual(x => x.HomeTeamId)
                .WithMessage("Home and Away teams must be different.");
            RuleFor(x => x.HomeGoals).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AwayGoals).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PlayedAt).LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1));
        }
    }

    public class CreateMatchCommandHandler(AppDbContext db) : IRequestHandler<CreateMatchCommand, MatchDto>
    {
        public async Task<MatchDto> Handle(CreateMatchCommand r, CancellationToken ct)
        {
            if (!await db.Teams.AnyAsync(t => t.Id == r.HomeTeamId, ct))
            {
                throw new KeyNotFoundException("Home team not found.");
            }
            if (!await db.Teams.AnyAsync(t => t.Id == r.AwayTeamId, ct))
            {
                throw new KeyNotFoundException("Away team not found.");
            }

            var entity = new Match
            {
                HomeTeamId = r.HomeTeamId,
                AwayTeamId = r.AwayTeamId,
                HomeGoals = r.HomeGoals,
                AwayGoals = r.AwayGoals,
                PlayedAt = r.PlayedAt.ToUniversalTime()
            };

            db.Matches.Add(entity);
            await db.SaveChangesAsync(ct);

            var home = await db.Teams.AsNoTracking().FirstAsync(t => t.Id == entity.HomeTeamId, ct);
            var away = await db.Teams.AsNoTracking().FirstAsync(t => t.Id == entity.AwayTeamId, ct);

            return new MatchDto
            {
                Id = entity.Id,
                HomeTeamId = entity.HomeTeamId,
                HomeTeamName = home.Name,
                AwayTeamId = entity.AwayTeamId,
                AwayTeamName = away.Name,
                HomeGoals = entity.HomeGoals,
                AwayGoals = entity.AwayGoals,
                PlayedAt = entity.PlayedAt,
                RowVersion = Convert.ToBase64String(entity.RowVersion)
            };
        }
    }
}
