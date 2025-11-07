using FootballRankings.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballRankings.Data.Data
{
    public static partial class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Lions", FoundedYear = 1901 },
            new Team { Id = 2, Name = "Eagles", FoundedYear = 1905 },
            new Team { Id = 3, Name = "Wolves", FoundedYear = 1910 }
        );
        }
    }
}
