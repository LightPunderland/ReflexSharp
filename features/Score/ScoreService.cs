

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

    public async Task<IEnumerable<ScoreEntity>> GetTopScoresAsync(int count)
    {
        return await _context.Scores
        .OrderByDescending(s => s.Score)
        .Take(count)
        .ToListAsync();
    }
}
