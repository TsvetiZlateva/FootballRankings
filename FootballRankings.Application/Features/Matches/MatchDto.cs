namespace FootballRankings.Application.Features.Matches
{
    public class MatchDto
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = "";
        public int AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = "";
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public DateTime PlayedAt { get; set; }
    }
}
