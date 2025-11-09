using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballRankings.Data.Models
{
    public class Team
    {
        public Team()
        {
            HomeTeamMatches = new HashSet<Match>();
            AyawTeamMatches = new HashSet<Match>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } 

        public int? FoundedYear { get; set; }

        [MaxLength(200)]
        public string? Coach { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public virtual ICollection<Match> HomeTeamMatches { get; set; }
        public virtual ICollection<Match> AyawTeamMatches { get; set; }
    }
}
