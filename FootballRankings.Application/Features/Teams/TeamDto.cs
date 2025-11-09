namespace FootballRankings.Application.Features.Teams
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? FoundedYear { get; set; }
        public string? Coach { get; set; }
        public string RowVersion { get; set; } = "";
    }
}
