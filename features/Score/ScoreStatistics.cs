public struct ScoreStatistics
{
    private int _scoreSum;  // Sum of all scores
    private int _scoreCount;  // Number of scores

    public ScoreStatistics(int scoreSum, int scoreCount)
    {
        _scoreSum = scoreSum;
        _scoreCount = scoreCount;
    }
    public void AddScore(int score)
    {
        _scoreSum += score;
        _scoreCount++;
    }

    public double GetAverageScore()
    {
        if (_scoreCount == 0) return 0; 
        return (double)_scoreSum / _scoreCount;
    }

    public int GetScoreCount() => _scoreCount;
}
