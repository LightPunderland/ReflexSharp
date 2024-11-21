using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data; 
using Features.Score; 

public class GenericRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly GenericRepository<ScoreEntity> _repository;

    public GenericRepositoryTests()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new GenericRepository<ScoreEntity>(_context);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var scores = new List<ScoreEntity>
        {
            new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 100 },
            new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 200 }
        };

        _context.Scores.AddRange(scores);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
    {
        
        var existingScore = _context.Scores.First();

        
        var result = await _repository.GetByIdAsync(existingScore.Id);

        
        Assert.NotNull(result);
        Assert.Equal(existingScore.Id, result.Id);
        Assert.Equal(existingScore.Score, result.Score);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
    {
        
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
       
        var result = await _repository.GetAllAsync();

       
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_AddsEntityToDatabase()
    {
       
        var newScore = new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 300 };

        
        await _repository.AddAsync(newScore);
        await _context.SaveChangesAsync();

        
        var result = await _context.Scores.FindAsync(newScore.Id);
        Assert.NotNull(result);
        Assert.Equal(newScore.Score, result.Score);
    }

    [Fact]
    public void Update_UpdatesEntityInDatabase()
    {
       
        var existingScore = _context.Scores.First();
        existingScore.Score = 500;

        
        _repository.Update(existingScore);
        _context.SaveChanges();

        
        var result = _context.Scores.Find(existingScore.Id);
        Assert.NotNull(result);
        Assert.Equal(500, result.Score);
    }

    [Fact]
    public void Delete_RemovesEntityFromDatabase()
    {
       
        var existingScore = _context.Scores.First();

        
        _repository.Delete(existingScore);
        _context.SaveChanges();

        
        var result = _context.Scores.Find(existingScore.Id);
        Assert.Null(result);
    }
}
