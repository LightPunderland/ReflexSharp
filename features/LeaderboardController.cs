using Microsoft.AspNetCore.Mvc;

namespace Features.Leaderboard
{

    [ApiController]
    [Route("api/leaderboard")]

    public class LeaderboardController : ControllerBase
    {
        public const int AMOUNT = 5; // Amount of users to be displayed with the highest score.

        private static List<LeaderboardEntry> Leaderboard = new List<LeaderboardEntry>
        {
            // Temporary testing data
            new LeaderboardEntry(0, "Vardenis", 9999),
            new LeaderboardEntry(1, "Pavardenis", 1001),
            new LeaderboardEntry(2, "Vardenis", 1),
            new LeaderboardEntry(3, "Auksine", 5),
            new LeaderboardEntry(4, "Raide", 50000),
        };

        [HttpGet]
        public IActionResult GetLeaderboard(){

            var topScores = Leaderboard
                .OrderByDescending(entry => entry.score)
                .Take(AMOUNT)
                .ToList();

            return Ok(topScores);
        }
    }
    public record LeaderboardEntry(int userID, string? username, int? score);
}
