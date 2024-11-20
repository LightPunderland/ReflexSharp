using Microsoft.AspNetCore.Mvc;

namespace features.Login
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        [HttpPost("google-signin")]
        public IActionResult GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            Console.WriteLine($"Received username: {request.Username}");
            Console.WriteLine($"Received token: {request.Token}");

            return Ok("Google Sign-In endpoint received the request successfully.");
        }

    }
}
