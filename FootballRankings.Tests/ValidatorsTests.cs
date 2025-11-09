using FootballRankings.Application.Features.Matches.Commands;
using FootballRankings.Application.Features.Teams.Commands;
using FluentValidation.TestHelper;

namespace FootballRankings.Tests
{
    public class ValidatorsTests
    {
        [Fact]
        public void CreateTeamValidator_Works()
        {
            var v = new CreateTeamCommandValidator();

            v.TestValidate(new CreateTeamCommand { Name = "" })
             .ShouldHaveValidationErrorFor(x => x.Name);

            v.TestValidate(new CreateTeamCommand { Name = new string('x', 201) })
             .ShouldHaveValidationErrorFor(x => x.Name);

            v.TestValidate(new CreateTeamCommand { Name = "OK", FoundedYear = 1700 })
             .ShouldHaveValidationErrorFor(x => x.FoundedYear);

            v.TestValidate(new CreateTeamCommand { Name = "OK", FoundedYear = 2000, Coach = "Alex" })
             .IsValid.Should().BeTrue();
        }

        [Fact]
        public void CreateMatchValidator_Works()
        {
            var v = new CreateMatchCommandValidator();
            var past = DateTime.UtcNow.AddMinutes(-5);

            v.TestValidate(new CreateMatchCommand { HomeTeamId = 1, AwayTeamId = 1, HomeGoals = 1, AwayGoals = 0, PlayedAt = past })
             .ShouldHaveValidationErrorFor(x => x.AwayTeamId);

            v.TestValidate(new CreateMatchCommand { HomeTeamId = 1, AwayTeamId = 2, HomeGoals = -1, AwayGoals = 0, PlayedAt = past })
             .ShouldHaveValidationErrorFor(x => x.HomeGoals);

            v.TestValidate(new CreateMatchCommand { HomeTeamId = 1, AwayTeamId = 2, HomeGoals = 1, AwayGoals = -1, PlayedAt = past })
             .ShouldHaveValidationErrorFor(x => x.AwayGoals);

            v.TestValidate(new CreateMatchCommand { HomeTeamId = 1, AwayTeamId = 2, HomeGoals = 2, AwayGoals = 1, PlayedAt = past })
             .IsValid.Should().BeTrue();
        }
    }
}
