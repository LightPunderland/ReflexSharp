using Features.Score;

public interface IScoreService
{
    Task<IEnumerable<ScoreEntity>> GetTopScoresAsync(int count);
}
