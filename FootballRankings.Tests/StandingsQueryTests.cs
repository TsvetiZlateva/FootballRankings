using FootballRankings.Application.Common;
using FootballRankings.Application.Features.Standings.Queries;
using FootballRankings.Data.Models;

namespace FootballRankings.Tests
{
    public class StandingsQueryTests
    {
        [Fact]
        public async Task Standings_Aggregate_Correctly()
        {
            using var db = TestFixture.CreateInMemoryDb();

            db.Matches.AddRange(
                new Match { HomeTeamId = 1, AwayTeamId = 2, HomeGoals = 2, AwayGoals = 1, PlayedAt = DateTime.UtcNow.AddDays(-3) }, // Lions win
                new Match { HomeTeamId = 2, AwayTeamId = 1, HomeGoals = 0, AwayGoals = 0, PlayedAt = DateTime.UtcNow.AddDays(-2) }, // draw
                new Match { HomeTeamId = 3, AwayTeamId = 1, HomeGoals = 1, AwayGoals = 4, PlayedAt = DateTime.UtcNow.AddDays(-1) }  // Lions win
            );
            await db.SaveChangesAsync();

            var handler = new GetStandingsQueryHandler(db, new ThreePointWinStrategy());
            var list = await handler.Handle(new GetStandingsQuery(), CancellationToken.None);

            var lions = list.Single(x => x.TeamName == "Lions");
            lions.Played.Should().Be(3);
            lions.Won.Should().Be(2);
            lions.Drawn.Should().Be(1);
            lions.Lost.Should().Be(0);
            lions.GoalsFor.Should().Be(6);
            lions.GoalsAgainst.Should().Be(2);
            lions.GoalDifference.Should().Be(4);
            lions.Points.Should().Be(7);
        }
    }
}
