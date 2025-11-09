using FootballRankings.Application.Features.Matches.Commands;
using FootballRankings.Application.Features.Matches.Queries;

namespace FootballRankings.Tests
{
    public class MatchesHandlersTests
    {
        [Fact]
        public async Task Create_Then_GetById_Works()
        {
            using var db = TestFixture.CreateInMemoryDb();

            var create = new CreateMatchCommand
            {
                HomeTeamId = 1,
                AwayTeamId = 2,
                HomeGoals = 3,
                AwayGoals = 2,
                PlayedAt = DateTime.UtcNow.AddDays(-1)
            };

            var created = await new CreateMatchCommandHandler(db).Handle(create, CancellationToken.None);

            created.Id.Should().BeGreaterThan(0);
            created.HomeTeamName.Should().Be("Lions");
            created.AwayTeamName.Should().Be("Eagles");

            var fetched = await new GetMatchByIdQueryHandler(db)
                .Handle(new GetMatchByIdQuery { Id = created.Id }, CancellationToken.None);

            fetched.Id.Should().Be(created.Id);
            fetched.HomeGoals.Should().Be(3);
            fetched.AwayGoals.Should().Be(2);
        }

        [Fact]
        public async Task CreateMatch_Works()
        {
            using var db = TestFixture.CreateInMemoryDb();

            var created = await new CreateMatchCommandHandler(db).Handle(
                new CreateMatchCommand
                {
                    HomeTeamId = 1,
                    AwayTeamId = 2,
                    HomeGoals = 2,
                    AwayGoals = 1,
                    PlayedAt = DateTime.UtcNow.AddDays(-1)
                },
                CancellationToken.None);

            created.Id.Should().BeGreaterThan(0);
            created.HomeTeamName.Should().Be("Lions");
            created.AwayTeamName.Should().Be("Eagles");
            created.RowVersion.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateMatch_Works()
        {
            using var db = TestFixture.CreateInMemoryDb();

            var created = await new CreateMatchCommandHandler(db).Handle(
                new CreateMatchCommand
                {
                    HomeTeamId = 1,
                    AwayTeamId = 2,
                    HomeGoals = 2,
                    AwayGoals = 1,
                    PlayedAt = DateTime.UtcNow.AddDays(-1)
                },
                CancellationToken.None);

            var updated = await new UpdateMatchCommandHandler(db).Handle(
                new UpdateMatchCommand
                {
                    Id = created.Id,
                    HomeGoals = 3,
                    AwayGoals = 2,
                    PlayedAt = DateTime.UtcNow.AddHours(-2),
                    RowVersion = created.RowVersion
                },
                CancellationToken.None);

            updated.HomeGoals.Should().Be(3);
            updated.AwayGoals.Should().Be(2);
        }

        [Fact]
        public async Task GetMatches_Pagination_Works()
        {
            using var db = TestFixture.CreateInMemoryDb();

            for (int i = 0; i < 25; i++)
            {
                db.Matches.Add(new Data.Models.Match
                {
                    HomeTeamId = 1,
                    AwayTeamId = 2,
                    HomeGoals = i % 5,
                    AwayGoals = (i + 1) % 5,
                    PlayedAt = DateTime.UtcNow.AddDays(-i)
                });
            }
            await db.SaveChangesAsync();

            var handler = new GetMatchesQueryHandler(db);

            var page1 = await handler.Handle(new GetMatchesQuery { Page = 1, PageSize = 10 }, CancellationToken.None);
            var page3 = await handler.Handle(new GetMatchesQuery { Page = 3, PageSize = 10 }, CancellationToken.None);

            page1.Items.Should().HaveCount(10);
            page1.Total.Should().Be(25);

            page3.Items.Should().HaveCount(5);
        }
    }
}
