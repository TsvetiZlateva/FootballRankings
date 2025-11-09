using FootballRankings.Application.Common.Interfaces;

namespace FootballRankings.Application.Common
{
    public class ThreePointWinStrategy : IScoringStrategy
    {
        public int PointsFor(int gf, int ga) => gf > ga ? 3 : gf == ga ? 1 : 0;
        public (int won, int drawn, int lost) OutcomeTally(int gf, int ga)
            => gf > ga ? (1, 0, 0) : gf == ga ? (0, 1, 0) : (0, 0, 1);
    }
}
