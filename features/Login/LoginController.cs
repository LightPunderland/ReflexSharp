using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace features.Login
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            try
            {
                var clientId = Environment.GetEnvironmentVariable("GOOGLE_API_CLIENT_ID");

                if (string.IsNullOrEmpty(clientId))
                {
                    return BadRequest("Google API Client ID is not set.");
                }

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }

                });

                Console.WriteLine($"Token is valid. User: {payload.Name}, Email: {payload.Email}");

                return Ok($"""Token validated successfully for {payload.Email} and username: "{request.Username}".""");
            }
            catch (InvalidJwtException e)
            {
                Console.WriteLine($"Invalid token: {e.Message}");
                return Unauthorized("Invalid token.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return StatusCode(500, "An error occurred while validating the token.");
            }

        }
    }
}
