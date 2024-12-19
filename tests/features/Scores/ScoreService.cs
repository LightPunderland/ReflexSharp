using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Features.Score;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Features.User.Entities;
using Xunit;

public class ScoreServiceTests
{
    private readonly Mock<IScoreRepository> _scoreRepositoryMock;
    private readonly AppDbContext _context;
    private readonly ScoreService _scoreService;

    public ScoreServiceTests()
    {
        _scoreRepositoryMock = new Mock<IScoreRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        _scoreService = new ScoreService(_scoreRepositoryMock.Object, _context);
    }

    [Fact]
    public async Task GetTopScoresAsync_ReturnsTopScores()
    {
        var testScores = new List<ScoreEntity>
        {
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 100 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 200 }
        };

        _scoreRepositoryMock
            .Setup(repo => repo.GetTopScoresAsync(5))
            .ReturnsAsync(testScores);

        var result = await _scoreService.GetTopScoresAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, score => score.Score == 100);
    }

    
 [Fact]
    public async Task CreateScoreAsync_AddsScoreAndClearsCache()
    {
        var userId = Guid.NewGuid();
        var score = 150;

        _scoreRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<ScoreEntity>()))
            .Callback<ScoreEntity>(entity =>
            {
                _context.Scores.Add(entity);
                _context.SaveChanges();
            })
            .Returns(Task.CompletedTask);

        var result = await _scoreService.CreateScoreAsync(userId, score);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(score, result.Score);

        var dbScore = await _context.Scores.FirstOrDefaultAsync(s => s.UserId == userId);
        Assert.NotNull(dbScore);
        Assert.Equal(score, dbScore.Score);
    }


    [Fact]
    public async Task CalculateAverageScoreAsync_ReturnsCorrectValue()
    {
        _context.Scores.AddRange(
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 100 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 200 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 300 }
        );
        await _context.SaveChangesAsync();

        var result = await _scoreService.CalculateAverageScoreAsync();

        Assert.Equal(200, result);
    }

    [Fact]
public async Task UserExistsAsync_ReturnsTrueIfUserExists()
{
    var userId = Guid.NewGuid();
    _context.Users.Add(new User
    {
        Id = userId,
        DisplayName = "Test User",
        Email = "testuser@example.com",
        GoogleId = "google123" 
    });
    await _context.SaveChangesAsync();

    var result = await _scoreService.UserExistsAsync(userId);

    Assert.True(result);
}


    [Fact]
    public async Task UserExistsAsync_ReturnsFalseIfUserDoesNotExist()
    {
        var userId = Guid.NewGuid();

        var result = await _scoreService.UserExistsAsync(userId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAverageScoreByUser_ReturnsCorrectAverage()
    {
        var userId = Guid.NewGuid();
        var scores = new List<ScoreEntity>
        {
            new ScoreEntity { UserId = userId, Score = 100 },
            new ScoreEntity { UserId = userId, Score = 200 }
        };

        _scoreRepositoryMock
            .Setup(repo => repo.GetScoresByUserAsync(userId))
            .ReturnsAsync(scores);

        var result = await _scoreService.GetAverageScoreByUser(userId);

        Assert.NotNull(result);
        Assert.IsType<ActionResult<double>>(result);
        Assert.Equal(150, result.Value);
    }

    [Fact]
    public async Task GetTopScoresbyUser_ReturnsTopScoresForUser()
    {
        var userId = Guid.NewGuid();
        var scores = new List<ScoreEntity>
        {
            new ScoreEntity { UserId = userId, Score = 300 },
            new ScoreEntity { UserId = userId, Score = 200 },
            new ScoreEntity { UserId = userId, Score = 100 }
        };

        _scoreRepositoryMock
            .Setup(repo => repo.GetTopScoresByUserAsync(userId, 2))
            .ReturnsAsync(scores.Take(2));

        var result = await _scoreService.GetTopScoresbyUser(userId, 2);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, score => score.Score == 300);
    }
}
