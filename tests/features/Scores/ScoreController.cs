using Xunit; 
using Moq; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc; 
using Features.Score; 

namespace Features.Score.Tests 
{
    public class ScoreControllerTests
    {
        private readonly Mock<IScoreService> _mockScoreService; 
        private readonly ScoreController _controller; 

        public ScoreControllerTests()
        {
            
            _mockScoreService = new Mock<IScoreService>();
            _controller = new ScoreController(_mockScoreService.Object);
        }


        [Fact]
        public async Task GetTopScores_ReturnsOkResult_WithScores()
        {
           
            var mockScores = new List<ScoreEntity>
            {
                new ScoreEntity { UserId = Guid.NewGuid(), Score = 100 },
                new ScoreEntity { UserId = Guid.NewGuid(), Score = 80 }
            };
            _mockScoreService.Setup(s => s.GetTopScoresAsync(It.IsAny<int>())).ReturnsAsync(mockScores);

            
            var result = await _controller.GetTopScores(2);

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnScores = Assert.IsAssignableFrom<IEnumerable<ScoreEntity>>(okResult.Value);
            Assert.Equal(2, returnScores.Count());
        }

        [Fact]
        public async Task GetTopScores_ReturnsNotFound_WhenNoScores()
        {
            
            _mockScoreService.Setup(s => s.GetTopScoresAsync(It.IsAny<int>())).ReturnsAsync(new List<ScoreEntity>());

            
            var result = await _controller.GetTopScores(2);

           
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateScore_ReturnsBadRequest_WhenModelStateInvalid()
        {
            
            _controller.ModelState.AddModelError("Score", "Required");

           
            var result = await _controller.CreateScore(new CreateScoreDto());

            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateScore_ReturnsNotFound_WhenUserDoesNotExist()
        {
            
            var dto = new CreateScoreDto { UserId = Guid.NewGuid(), Score = 50 };
            _mockScoreService.Setup(s => s.UserExistsAsync(dto.UserId)).ReturnsAsync(false);

            
            var result = await _controller.CreateScore(dto);

            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("User not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetAverageScore_ReturnsOkResult_WithAverageScore()
        {
            
            double averageScore = 75.5;
            _mockScoreService.Setup(s => s.CalculateAverageScoreAsync()).ReturnsAsync(averageScore);

            
            var result = await _controller.GetAverageScore();

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(averageScore, okResult.Value);
        }

        [Fact]
        public async Task GetAverageScoreByUser_ReturnsInternalServerError_OnException()
        {
            
            _mockScoreService.Setup(s => s.GetAverageScoreByUser(It.IsAny<Guid>())).ThrowsAsync(new Exception("Test Exception"));

           
            var result = await _controller.GetAverageScoreByUser(Guid.NewGuid());

            
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Test Exception", statusCodeResult.Value.ToString());
        }
    }
}
