

using Data;
using Features.Score;
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
}
