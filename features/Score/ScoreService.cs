using Data;
using Features.Score;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ScoreService : IScoreService
{
    private readonly AppDbContext _context;
    
    // In-memory cache for storing values with boxing/unboxing
    private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

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

        // Optional: You can invalidate cache related to scores if necessary

        return scoreEntity;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    // Method to calculate the average score across all users (with caching)
    public async Task<double> CalculateAverageScoreAsync()
    {
        string cacheKey = "AverageScore";

        // Check if average score is cached (unboxing occurs here)
        if (_cache.ContainsKey(cacheKey))
        {
            return (double)_cache[cacheKey]; // Unboxing
        }

        // Calculate the average score if not cached
        var scores = await _context.Scores.ToListAsync();
        var scoreStats = new ScoreStatistics(0, 0);

        foreach (var score in scores)
        {
            scoreStats.AddScore(score.Score);
        }

        double averageScore = scoreStats.GetAverageScore();

        // Store the result in the cache (boxing occurs here)
        _cache[cacheKey] = averageScore; // Boxing

        return averageScore;
    }

    // Method to calculate the average score by user (with caching)
    public async Task<ActionResult<double>> GetAverageScoreByUser(Guid userId)
    {
        string cacheKey = $"AverageScore_{userId}";

        // Check if the user's average score is cached (unboxing occurs here)
        if (_cache.ContainsKey(cacheKey))
        {
            return (double)_cache[cacheKey]; // Unboxing
        }

        // Calculate the user's average score if not cached
        var userScores = await _context.Scores
            .Where(s => s.UserId == userId)
            .ToListAsync();

        var scoreStats = new ScoreStatistics(0, 0);

        foreach (var score in userScores)
        {
            scoreStats.AddScore(score.Score);
        }

        double averageScore = scoreStats.GetAverageScore();

        // Store the result in the cache (boxing occurs here)
        _cache[cacheKey] = averageScore; // Boxing

        return averageScore;
    }
}
