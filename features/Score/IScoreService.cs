using Features.Score;

public interface IScoreService
{
    Task<IEnumerable<ScoreEntity>> GetTopScoresAsync(int count);
    Task<IEnumerable<ScoreEntity>> GetTopScoresbyUser(Guid guid, int count);

    Task<ScoreEntity> CreateScoreAsync(Guid userId, int score);
    Task<bool> UserExistsAsync(Guid userId);
}
