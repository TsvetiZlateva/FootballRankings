using FluentAssertions;
using FootballRankings.Application.Common;

namespace FootballRankings.Tests
{
    public class ThreePointWinStrategyTests
    {
        [Fact]
        public void PointsAndOutcomes_AreCorrect()
        {
            var s = new ThreePointWinStrategy();

            s.PointsFor(2, 1).Should().Be(3);
            s.PointsFor(1, 1).Should().Be(1);
            s.PointsFor(0, 1).Should().Be(0);

            s.OutcomeTally(3, 1).Should().Be((1, 0, 0));
            s.OutcomeTally(2, 2).Should().Be((0, 1, 0));
            s.OutcomeTally(0, 4).Should().Be((0, 0, 1));
        }
    }
}
