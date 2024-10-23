

using Features.Score;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/leaderboard")]

public class ScoreController : ControllerBase
{
    private readonly IScoreService _scoreService;

    public ScoreController(IScoreService scoreService)
    {
        _scoreService = scoreService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScoreEntity>>> GetTopScores(int count)
    {

        if (count <= 0)
        {
            return BadRequest("Count must be greater than 0");
        }

        try
        {
            var scores = await _scoreService.GetTopScoresAsync(count);

            if (scores == null || !scores.Any())
            {
                return NotFound();
            }

            return Ok(scores);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<ScoreEntity>>> GetTopScoresByUser(Guid userId, int count)
    {
        if (count <= 0)
        {
            return BadRequest("Count must be greater than 0");
        }

        try
        {
            var scores = await _scoreService.GetTopScoresbyUser(userId, count);

            if (scores == null || !scores.Any())
            {
                return NotFound();
            }

            return Ok(scores);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateScore([FromBody] CreateScoreDto createScoreDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (createScoreDto.Score < 0)
        {
            return BadRequest(new { error = "Score must be greater than or equal to 0" });
        }

        bool userExists = await _scoreService.UserExistsAsync(createScoreDto.UserId);
        if (!userExists)
        {
            return NotFound(new { error = "User not found" });
        }

        var newScore = await _scoreService.CreateScoreAsync(createScoreDto.UserId, createScoreDto.Score);
        if (newScore == null)
        {
            return StatusCode(500, new { error = "An error occurred while creating the score" });
        }


        return Ok(newScore);
    }

    [HttpGet("average")]
    public async Task<ActionResult<double>> GetAverageScore()
{
    try
    {
        var averageScore = await _scoreService.CalculateAverageScoreAsync();
        return Ok(averageScore);
    }
    catch (Exception e)
    {
        return StatusCode(500, e.Message);
    }
}

    [HttpGet("average/{userId}")]
    public async Task<ActionResult<double>> GetAverageScoreByUser(Guid userId)
{
    try
    {
        var averageScore = await _scoreService.GetAverageScoreByUser(userId);
        return Ok(averageScore);
    }
    catch (Exception e)
    {
        return StatusCode(500, e.Message);
    }
}

}


