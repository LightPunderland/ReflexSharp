using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Features.Score; 
using Data; 

namespace Features.Score.Tests 
{
    public class ScoreRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly ScoreRepository _repository;

        public ScoreRepositoryTests()
        {
            
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            _context = new AppDbContext(options);
            _repository = new ScoreRepository(_context);

            
            SeedDatabase();
        }

        
        private void SeedDatabase()
        {
            var scores = new List<ScoreEntity>
            {
                new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 100 },
                new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 200 },
                new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 50 },
                new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 300 },
                new ScoreEntity { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Score = 150 }
            };

            _context.Scores.AddRange(scores);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetTopScoresAsync_ReturnsTopScores()
        {
            
            var result = await _repository.GetTopScoresAsync(3);

            
            Assert.Equal(3, result.Count());
            Assert.Equal(300, result.First().Score);
        }

        [Fact]
        public async Task GetTopScoresByUserAsync_ReturnsUserTopScores()
        {
           
            var userId = _context.Scores.First().UserId;

            
            var result = await _repository.GetTopScoresByUserAsync(userId, 2);

            
            Assert.NotNull(result);
            Assert.All(result, score => Assert.Equal(userId, score.UserId));
            Assert.True(result.Count() <= 2); 
        }

        [Fact]
        public async Task GetScoresByUserAsync_ReturnsAllScoresForUser()
        {
           
            var userId = _context.Scores.First().UserId;

            
            var result = await _repository.GetScoresByUserAsync(userId);

            
            Assert.NotNull(result);
            Assert.All(result, score => Assert.Equal(userId, score.UserId));
        }

        [Fact]
        public async Task GetTopScoresAsync_ReturnsEmpty_WhenNoScores()
        {
            
            var emptyContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var emptyContext = new AppDbContext(emptyContextOptions);
            var emptyRepository = new ScoreRepository(emptyContext);

            
            var result = await emptyRepository.GetTopScoresAsync(5);

           
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetScoresByUserAsync_ReturnsEmpty_WhenUserHasNoScores()
        {
            
            var result = await _repository.GetScoresByUserAsync(Guid.NewGuid()); 

           
            Assert.Empty(result);
        }
    }
}
