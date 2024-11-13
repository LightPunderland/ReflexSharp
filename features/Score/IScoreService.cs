using Features.Score;
using Microsoft.AspNetCore.Mvc;

public interface IScoreService
{
    Task<IEnumerable<ScoreEntity>> GetTopScoresAsync(int count = 5);
    Task<IEnumerable<ScoreEntity>> GetTopScoresByUser(Guid guid, int count = 5);

    Task<ScoreEntity> CreateScoreAsync(Guid userId, int score);
    Task<bool> UserExistsAsync(Guid userId);

    Task<double> CalculateAverageScoreAsync();

    Task<ActionResult<double>> GetAverageScoreByUser(Guid userId);
}
