using Data;
using Features.Score;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

public class ScoreService : IScoreService
{
    private readonly AppDbContext _context;

    // Dictionary pakeistas į ConcurrentDictionary 
    private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

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

        // Pakeičia sena average scora su updatintu
        _cache.TryRemove("AverageScore", out _);
        _cache.TryRemove($"AverageScore_{userId}", out _);

        return scoreEntity;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    // Apskaičiuoja average scora 
    public async Task<double> CalculateAverageScoreAsync()
    {
        string cacheKey = "AverageScore";

        
        if (_cache.TryGetValue(cacheKey, out var cachedValue))
        {
            return (double)cachedValue;
        }

        
        var scores = await _context.Scores.ToListAsync();

        // Task.run sukuria nauja threada backgrounde (turėtų skaitytis kaip multithread)
        double averageScore = await Task.Run(() => {
            var scoreStats = new ScoreStatistics(0, 0);
            foreach (var score in scores)
            {
                scoreStats.AddScore(score.Score);
            }
            return scoreStats.GetAverageScore();
        });

        _cache[cacheKey] = averageScore;

        return averageScore;
    }

    // Suskaičiuoja average score kiekvieno userio
    public async Task<ActionResult<double>> GetAverageScoreByUser(Guid userId)
    {
        string cacheKey = $"AverageScore_{userId}";

        
        if (_cache.TryGetValue(cacheKey, out var cachedValue))
        {
            return (double)cachedValue;
        }

        var userScores = await _context.Scores
            .Where(s => s.UserId == userId)
            .ToListAsync();

        double averageScore = await Task.Run(() => {
            var scoreStats = new ScoreStatistics(0, 0);
            foreach (var score in userScores)
            {
                scoreStats.AddScore(score.Score);
            }
            return scoreStats.GetAverageScore();
        });

        _cache[cacheKey] = averageScore;

        return averageScore;
    }
}
