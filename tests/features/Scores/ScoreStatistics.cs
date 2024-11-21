using Xunit;

namespace Features.Score.Tests 
{
    public class ScoreStatisticsTests
    {
        [Fact]
        public void Constructor_InitializesValuesCorrectly()
        {
            
            int scoreSum = 10;
            int scoreCount = 2;

            
            var statistics = new ScoreStatistics(scoreSum, scoreCount);

            
            Assert.Equal(2, statistics.GetScoreCount());
            Assert.Equal(5, statistics.GetAverageScore());
        }

        [Fact]
        public void AddScore_UpdatesSumAndCountCorrectly()
        {
            
            var statistics = new ScoreStatistics(10, 2);

            
            statistics.AddScore(20);

            
            Assert.Equal(3, statistics.GetScoreCount());
            Assert.Equal(10, statistics.GetAverageScore()); 
        }

        [Fact]
        public void GetAverageScore_ReturnsZero_WhenNoScores()
        {
            
            var statistics = new ScoreStatistics(0, 0);

            
            var average = statistics.GetAverageScore();

           
            Assert.Equal(0, average);
        }

        [Fact]
        public void GetAverageScore_ReturnsCorrectValue_WhenScoresAreAdded()
        {
            
            var statistics = new ScoreStatistics(0, 0);

            
            statistics.AddScore(10);
            statistics.AddScore(20);
            statistics.AddScore(30);

            
            Assert.Equal(3, statistics.GetScoreCount());
            Assert.Equal(20, statistics.GetAverageScore());
        }

        [Fact]
        public void GetScoreCount_ReturnsCorrectCount()
        {
           
            var statistics = new ScoreStatistics(0, 0);

            
            statistics.AddScore(15);
            statistics.AddScore(25);

            
            Assert.Equal(2, statistics.GetScoreCount());
        }
    }
}
