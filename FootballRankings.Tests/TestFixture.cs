using FootballRankings.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Tests;

public static class TestFixture
{
    public static AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"FootballRankings_{Guid.NewGuid()}")
            .Options;

        var db = new AppDbContext(options);

        if (!db.Teams.Any())
        {
            db.Teams.AddRange(
                new Data.Models.Team { Id = 1, Name = "Lions", FoundedYear = 1901, Coach = "M. Carter" },
                new Data.Models.Team { Id = 2, Name = "Eagles", FoundedYear = 1905, Coach = "S. Ruiz" },
                new Data.Models.Team { Id = 3, Name = "Wolves", FoundedYear = 1910, Coach = "D. Novak" }
            );
            db.SaveChanges();
        }

        return db;
    }
}
