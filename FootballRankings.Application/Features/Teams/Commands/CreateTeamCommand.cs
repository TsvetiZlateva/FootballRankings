using FluentValidation;
using FootballRankings.Data.Data;
using FootballRankings.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Teams.Commands
{
    public record CreateTeamCommand : IRequest<TeamDto>
    {
        public string Name { get; init; }
        public int? FoundedYear { get; init; }
        public string? Coach { get; init; }
    }

    public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        public CreateTeamCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.FoundedYear).GreaterThan(1800).When(x => x.FoundedYear.HasValue);
            RuleFor(x => x.Coach).MaximumLength(200).When(x => x.Coach != null);
        }
    }

    public class CreateTeamCommandHandler(AppDbContext db) : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        public async Task<TeamDto> Handle(CreateTeamCommand r, CancellationToken ct)
        {
            if (await db.Teams.AnyAsync(t => t.Name == r.Name, ct))
            {
                throw new InvalidOperationException("Team with this name already exists.");
            }

            var team = new Team 
            { 
                Name = r.Name.Trim(), 
                FoundedYear = r.FoundedYear, 
                Coach = r.Coach 
            };

            db.Teams.Add(team);
            await db.SaveChangesAsync(ct);

            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                FoundedYear = team.FoundedYear,
                Coach = team.Coach
            };
        }
    }
}
