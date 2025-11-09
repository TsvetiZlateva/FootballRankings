namespace FootballRankings.Application.Common.Interfaces
{
    public interface IScoringStrategy
    {
        int PointsFor(int goalsFor, int goalsAgainst);
        (int won, int drawn, int lost) OutcomeTally(int gf, int ga);
    }
}
