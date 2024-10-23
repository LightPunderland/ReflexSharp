

using Data;
using Features.Score;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ScoreService : IScoreService
{
    private readonly AppDbContext _context;

    public ScoreService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ScoreEntity>> GetTopScoresAsync(int count = 5)
    {
        return await _context.Scores
        .OrderByDescending(s => s.Score)
        .Take(count)
        .ToListAsync();
    }

    public async Task<IEnumerable<ScoreEntity>> GetTopScoresbyUser(Guid guid, int count = 5)
    {
        return await _context.Scores
        .Where(s => s.UserId == guid)
        .OrderByDescending(s => s.Score)
        .Take(count)
        .ToListAsync();
    }

    public async Task<ScoreEntity> CreateScoreAsync(Guid userId, int score)
    {
        var scoreEntity = new ScoreEntity
        {
            UserId = userId,
            Score = score
        };

        _context.Scores.Add(scoreEntity);
        await _context.SaveChangesAsync();

        return scoreEntity;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    // Method to calculate the average score across all users
    public async Task<double> CalculateAverageScoreAsync()
    {
        var scores = await _context.Scores.ToListAsync();
        var scoreStats = new ScoreStatistics(0, 0); 

        foreach (var score in scores)
        {
            scoreStats.AddScore(score.Score);
        }

        return scoreStats.GetAverageScore();
    }

    // Method to calculate the average score for a specific user
    public async Task<double> GetAverageScoreByUser(Guid userId)
    {
        var userScores = await _context.Scores
            .Where(s => s.UserId == userId)
            .ToListAsync();

        var scoreStats = new ScoreStatistics(0, 0); 

        foreach (var score in userScores)
        {
            scoreStats.AddScore(score.Score); 
        }

        return scoreStats.GetAverageScore(); 
    }

    Task<ActionResult<double>> IScoreService.GetAverageScoreByUser(Guid userId)
    {
        throw new NotImplementedException();
    }
}
