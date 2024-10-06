

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

}


