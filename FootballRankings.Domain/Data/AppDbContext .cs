using FootballRankings.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballRankings.Data.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Match> Matches => Set<Match>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(b =>
            {
                b.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<Match>(b =>
            {
                b.HasOne(x => x.HomeTeam)
                 .WithMany(y => y.HomeTeamMatches)
                 .HasForeignKey(x => x.HomeTeamId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.AwayTeam)
                 .WithMany(y => y.AyawTeamMatches)
                 .HasForeignKey(x => x.AwayTeamId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => new { x.HomeTeamId, x.AwayTeamId, x.PlayedAt });
            });

            modelBuilder.Seed();
        }
    }
}
