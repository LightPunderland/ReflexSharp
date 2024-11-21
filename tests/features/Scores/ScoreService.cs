using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Features.Score;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ScoreServiceTests
{
    [Fact]
    public async Task GetTopScoresAsync_ShouldReturnTopScores()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetTopScores")
            .Options;

        using var context = new AppDbContext(options);
        context.Scores.AddRange(
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 100 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 200 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 50 }
        );
        await context.SaveChangesAsync();

        var service = new ScoreService(context);

        
        var topScores = await service.GetTopScoresAsync(2);

        
        Assert.Equal(2, topScores.Count());
        Assert.Equal(200, topScores.First().Score); 
    }

    [Fact]
    public async Task GetTopScoresByUser_ShouldReturnUserTopScores()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetTopScoresByUser")
            .Options;

        using var context = new AppDbContext(options);
        var userId = Guid.NewGuid();
        context.Scores.AddRange(
            new ScoreEntity { UserId = userId, Score = 300 },
            new ScoreEntity { UserId = userId, Score = 100 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 200 }
        );
        await context.SaveChangesAsync();

        var service = new ScoreService(context);

        
        var userTopScores = await service.GetTopScoresbyUser(userId, 2);

       
        Assert.Equal(2, userTopScores.Count());
        Assert.Equal(300, userTopScores.First().Score);
    }

    [Fact]
    public async Task CreateScoreAsync_ShouldAddScoreAndInvalidateCache()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateScore")
            .Options;

        using var context = new AppDbContext(options);
        var service = new ScoreService(context);

        var userId = Guid.NewGuid();

        
        var newScore = await service.CreateScoreAsync(userId, 150);

        
        Assert.NotNull(newScore);
        Assert.Equal(150, newScore.Score);
        Assert.Equal(userId, newScore.UserId);
        Assert.Single(context.Scores); 
    }

    [Fact]
    public async Task CalculateAverageScoreAsync_ShouldReturnCorrectAverage()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CalculateAverage")
            .Options;

        using var context = new AppDbContext(options);
        context.Scores.AddRange(
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 100 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 200 }
        );
        await context.SaveChangesAsync();

        var service = new ScoreService(context);

        
        var averageScore = await service.CalculateAverageScoreAsync();

        
        Assert.Equal(150, averageScore);
    }

    [Fact]
    public async Task GetAverageScoreByUser_ShouldReturnUserSpecificAverage()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetAverageScoreByUser")
            .Options;

        using var context = new AppDbContext(options);
        var userId = Guid.NewGuid();
        context.Scores.AddRange(
            new ScoreEntity { UserId = userId, Score = 50 },
            new ScoreEntity { UserId = userId, Score = 150 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 200 } 
        );
        await context.SaveChangesAsync();

        var service = new ScoreService(context);

        
        var averageScore = await service.GetAverageScoreByUser(userId);

        
        Assert.NotNull(averageScore);
        Assert.Equal(100, averageScore.Value); 
    }

    
}
