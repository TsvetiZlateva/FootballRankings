using FluentValidation;
using FootballRankings.Data.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Application.Features.Teams.Commands
{
    public record UpdateTeamCommand : IRequest<TeamDto>
    {
        public int Id { get; set; }                 
        public string Name { get; init; } = "";
        public int? FoundedYear { get; init; }
        public string? Coach { get; init; }
        public string RowVersion { get; init; } = ""; 
    }

    public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
    {
        public UpdateTeamCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.RowVersion).NotEmpty();
            RuleFor(x => x.FoundedYear).GreaterThan(1800).When(x => x.FoundedYear.HasValue);
            RuleFor(x => x.Coach).MaximumLength(200).When(x => x.Coach != null);
        }
    }

    public class UpdateTeamCommandHandler(AppDbContext db) : IRequestHandler<UpdateTeamCommand, TeamDto>
    {
        public async Task<TeamDto> Handle(UpdateTeamCommand r, CancellationToken ct)
        {
            var teamToBeUpdated = await db.Teams.FirstOrDefaultAsync(t => t.Id == r.Id, ct);

            if (teamToBeUpdated == null)
            {
                throw new KeyNotFoundException("Team not found.");
            }

            db.Entry(teamToBeUpdated).Property(x => x.RowVersion).OriginalValue = Convert.FromBase64String(r.RowVersion);

            if (await db.Teams.AnyAsync(t => t.Id != r.Id && t.Name == r.Name, ct))
            {
                throw new InvalidOperationException("Another team already uses this name.");
            }

            teamToBeUpdated.Name = r.Name.Trim();
            teamToBeUpdated.FoundedYear = r.FoundedYear;
            teamToBeUpdated.Coach = r.Coach;

            await db.SaveChangesAsync(ct);

            return new TeamDto
            {
                Id = teamToBeUpdated.Id,
                Name = teamToBeUpdated.Name,
                FoundedYear = teamToBeUpdated.FoundedYear,
                Coach = teamToBeUpdated.Coach,
                RowVersion = Convert.ToBase64String(teamToBeUpdated.RowVersion)
            };
        }
    }
}
