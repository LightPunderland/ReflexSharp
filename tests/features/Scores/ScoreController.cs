using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Features.Score; 
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class ScoreControllerTests
{
    [Fact]
    public async Task GetTopScores_ReturnsOk_WhenScoresExist_AndCountIsZero()
    {
        var mockService = new Mock<IScoreService>();
        var scores = new List<ScoreEntity>
        {
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 100 },
            new ScoreEntity { UserId = Guid.NewGuid(), Score = 200 }
        };

        // Use It.IsAny<int>() to avoid optional argument issues
        mockService.Setup(s => s.GetTopScoresAsync(It.IsAny<int>())).ReturnsAsync(scores);

        var controller = new ScoreController(mockService.Object);
        var actionResult = await controller.GetTopScores(0); 

        var okResult = actionResult.Result as OkObjectResult;
        Assert.NotNull(okResult); // If this fails, ensure service returns non-empty scores.
        var returnedScores = Assert.IsType<List<ScoreEntity>>(okResult.Value);
        Assert.Equal(2, returnedScores.Count);
    }

    [Fact]
    public async Task GetTopScores_ReturnsOk_WhenScoresExist_AndCountIsFive()
    {
        var mockService = new Mock<IScoreService>();
        var scores = new List<ScoreEntity> { new ScoreEntity { UserId = Guid.NewGuid(), Score = 100 } };

        mockService.Setup(s => s.GetTopScoresAsync(It.IsAny<int>())).ReturnsAsync(scores);

        var controller = new ScoreController(mockService.Object);
        var actionResult = await controller.GetTopScores(5);

        var okResult = actionResult.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedScores = Assert.IsType<List<ScoreEntity>>(okResult.Value);
        Assert.Single(returnedScores);
    }

    [Fact]
    public async Task GetTopScores_ReturnsNotFound_WhenNoScores()
    {
        var mockService = new Mock<IScoreService>();
        mockService.Setup(s => s.GetTopScoresAsync(It.IsAny<int>())).ReturnsAsync(Enumerable.Empty<ScoreEntity>());

        var controller = new ScoreController(mockService.Object);
        var actionResult = await controller.GetTopScores(0);

        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task GetTopScoresByUser_ReturnsOk_WhenScoresExistForUser_AndCountIsZero()
    {
        var mockService = new Mock<IScoreService>();
        var userId = Guid.NewGuid();
        var scores = new List<ScoreEntity> { new ScoreEntity { UserId = userId, Score = 300 } };

        mockService.Setup(s => s.GetTopScoresbyUser(userId, It.IsAny<int>())).ReturnsAsync(scores);

        var controller = new ScoreController(mockService.Object);
        var actionResult = await controller.GetTopScoresByUser(userId, 0);

        var okResult = actionResult.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedScores = Assert.IsType<List<ScoreEntity>>(okResult.Value);
        Assert.Single(returnedScores);
    }

    [Fact]
    public async Task GetTopScoresByUser_ReturnsOk_WhenScoresExistForUser_AndCountIsThree()
    {
        var mockService = new Mock<IScoreService>();
        var userId = Guid.NewGuid();
        var scores = new List<ScoreEntity> { new ScoreEntity { UserId = userId, Score = 300 } };

        mockService.Setup(s => s.GetTopScoresbyUser(userId, It.IsAny<int>())).ReturnsAsync(scores);

        var controller = new ScoreController(mockService.Object);
        var actionResult = await controller.GetTopScoresByUser(userId, 3);

        var okResult = actionResult.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedScores = Assert.IsType<List<ScoreEntity>>(okResult.Value);
        Assert.Single(returnedScores);
    }

    [Fact]
    public async Task GetTopScoresByUser_ReturnsNotFound_WhenNoScoresForUser()
    {
        var mockService = new Mock<IScoreService>();
        var userId = Guid.NewGuid();

        mockService.Setup(s => s.GetTopScoresbyUser(userId, It.IsAny<int>())).ReturnsAsync(Enumerable.Empty<ScoreEntity>());

        var controller = new ScoreController(mockService.Object);
        var actionResult = await controller.GetTopScoresByUser(userId, 0);

        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task CreateScore_ReturnsBadRequest_WhenModelInvalid()
    {
        var mockService = new Mock<IScoreService>();
        var controller = new ScoreController(mockService.Object);
        controller.ModelState.AddModelError("Score", "Required");

        var dto = new CreateScoreDto();
        var result = await controller.CreateScore(dto);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateScore_ReturnsBadRequest_WhenScoreNegative()
    {
        var mockService = new Mock<IScoreService>();
        var controller = new ScoreController(mockService.Object);

        var dto = new CreateScoreDto { Score = -10, UserId = Guid.NewGuid() };
        var result = await controller.CreateScore(dto) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Score must be greater than or equal to 0", result.Value.ToString());
    }

    [Fact]
    public async Task CreateScore_ReturnsNotFound_WhenUserNotExists()
    {
        var mockService = new Mock<IScoreService>();
        mockService.Setup(s => s.UserExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var controller = new ScoreController(mockService.Object);
        var dto = new CreateScoreDto { UserId = Guid.NewGuid(), Score = 100 };

        var result = await controller.CreateScore(dto) as NotFoundObjectResult;
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task CreateScore_ReturnsOk_WhenScoreCreated()
    {
        var mockService = new Mock<IScoreService>();
        var userId = Guid.NewGuid();
        var newScore = new ScoreEntity { UserId = userId, Score = 100 };

        mockService.Setup(s => s.UserExistsAsync(userId)).ReturnsAsync(true);
        mockService.Setup(s => s.CreateScoreAsync(userId, 100)).ReturnsAsync(newScore);

        var controller = new ScoreController(mockService.Object);
        var dto = new CreateScoreDto { UserId = userId, Score = 100 };

        var result = await controller.CreateScore(dto) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var returnedScore = Assert.IsType<ScoreEntity>(result.Value);
        Assert.Equal(100, returnedScore.Score);
    }

    [Fact]
    public async Task CreateScore_Returns500_WhenCreationFails()
    {
        var mockService = new Mock<IScoreService>();
        var userId = Guid.NewGuid();

        mockService.Setup(s => s.UserExistsAsync(userId)).ReturnsAsync(true);
        mockService.Setup(s => s.CreateScoreAsync(userId, 100)).ReturnsAsync((ScoreEntity?)null);

        var controller = new ScoreController(mockService.Object);
        var dto = new CreateScoreDto { UserId = userId, Score = 100 };

        var result = await controller.CreateScore(dto) as ObjectResult;
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task GetAverageScore_ReturnsOk()
    {
        var mockService = new Mock<IScoreService>();
        mockService.Setup(s => s.CalculateAverageScoreAsync()).ReturnsAsync(75.5);

        var controller = new ScoreController(mockService.Object);
        ActionResult<double> actionResult = await controller.GetAverageScore();

        var okResult = actionResult.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var value = Assert.IsType<double>(okResult.Value);
        Assert.Equal(75.5, value);
    }

    [Fact]
    public async Task GetAverageScore_Returns500_OnException()
    {
        var mockService = new Mock<IScoreService>();
        mockService.Setup(s => s.CalculateAverageScoreAsync()).ThrowsAsync(new Exception("Some error"));

        var controller = new ScoreController(mockService.Object);
        ActionResult<double> actionResult = await controller.GetAverageScore();

        var objectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Some error", objectResult.Value);
    }

    [Fact]
    public async Task GetAverageScoreByUser_ReturnsOk()
    {
        var mockService = new Mock<IScoreService>();
        var userId = Guid.NewGuid();
        mockService.Setup(s => s.GetAverageScoreByUser(userId)).ReturnsAsync(60.0);

        var controller = new ScoreController(mockService.Object);
        ActionResult<double> actionResult = await controller.GetAverageScoreByUser(userId);

        if (actionResult.Result == null)
        {
            var value = Assert.IsType<double>(actionResult.Value);
            Assert.Equal(60.0, value);
        }
        else
        {
            var okResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okResult);

            if (okResult.Value is ActionResult<double> innerAction)
            {
            
                if (innerAction.Result is OkObjectResult innerOk)
                {
                    var finalValue = Assert.IsType<double>(innerOk.Value);
                    Assert.Equal(60.0, finalValue);
                }
                else
                {
                    Assert.Equal(60.0, innerAction.Value);
                }
            }
            else
            {
                var value = Assert.IsType<double>(okResult.Value);
                Assert.Equal(60.0, value);
            }
        }
    }

    [Fact]
    public async Task GetAverageScoreByUser_Returns500_OnException()
    {
        var mockService = new Mock<IScoreService>();
        var userId = Guid.NewGuid();
        mockService.Setup(s => s.GetAverageScoreByUser(userId)).ThrowsAsync(new Exception("User average error"));

        var controller = new ScoreController(mockService.Object);
        ActionResult<double> actionResult = await controller.GetAverageScoreByUser(userId);

        var objectResult = actionResult.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("User average error", objectResult.Value);
    }
}
