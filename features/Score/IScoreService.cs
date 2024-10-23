using Features.Score;

public interface IScoreService
{
    Task<IEnumerable<ScoreEntity>> GetTopScoresAsync(int count = 5);
    Task<IEnumerable<ScoreEntity>> GetTopScoresbyUser(Guid guid, int count = 5);

    Task<ScoreEntity> CreateScoreAsync(Guid userId, int score);
    Task<bool> UserExistsAsync(Guid userId);
}
