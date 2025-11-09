using FootballRankings.Application.Features.Teams.Commands;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Tests
{
    public class TeamsHandlersTests
    {
        [Fact]
        public async Task CreateTeam_Works()
        {
            using var db = TestFixture.CreateInMemoryDb();

            var created = await new CreateTeamCommandHandler(db).Handle(
                new CreateTeamCommand { Name = "Tigers", FoundedYear = 1920, Coach = "Alex" },
                CancellationToken.None);

            created.Id.Should().BeGreaterThan(0);
            created.Name.Should().Be("Tigers");
            created.RowVersion.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateTeam_Works()
        {
            using var db = TestFixture.CreateInMemoryDb();

            // Arrange: create first so we have a real Id + rowVersion
            var created = await new CreateTeamCommandHandler(db).Handle(
                new CreateTeamCommand { Name = "Tigers", FoundedYear = 1920, Coach = "Alex" },
                CancellationToken.None);

            // Act
            var updated = await new UpdateTeamCommandHandler(db).Handle(
                new UpdateTeamCommand
                {
                    Id = created.Id,
                    Name = "Tigers FC",
                    FoundedYear = 1920,
                    Coach = "Alex",
                    RowVersion = created.RowVersion
                },
                CancellationToken.None);

            // Assert
            updated.Id.Should().Be(created.Id);
            updated.Name.Should().Be("Tigers FC");
        }

        [Fact]
        public async Task Delete_Team_Without_Matches_Works()
        {
            using var db = TestFixture.CreateInMemoryDb();

            var created = await new CreateTeamCommandHandler(db)
                .Handle(new CreateTeamCommand { Name = "Panthers" }, CancellationToken.None);

            await new DeleteTeamCommandHandler(db)
                .Handle(new DeleteTeamCommand { Id = created.Id }, CancellationToken.None);

            (await db.Teams.AnyAsync(t => t.Id == created.Id)).Should().BeFalse();
        }
    }
}
