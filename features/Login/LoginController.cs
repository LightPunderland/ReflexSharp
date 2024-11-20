using Features.User.DTOs;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace features.Login
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

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

                var googleId = payload.Subject;
                var email = payload.Email;
                var name = payload.Name;

                var existingUser = await _userService.ValidateUserAsync(googleId, email, request.Username);
                if (existingUser != null)
                {
                    // User exists
                    return Ok($"User validated successfully. Welcome back, {existingUser.DisplayName}!");
                }

                // Validate username uniqueness
                var isUsernameTaken = await _userService.IsUsernameTakenAsync(request.Username);
                if (isUsernameTaken)
                {
                    return Conflict("Username is already in use. Please choose a different username.");
                }

                // Validate email uniqueness
                var isEmailInUse = await _userService.IsEmailTakenAsync(email);
                if (isEmailInUse)
                {
                    return Conflict("Email is already in use. Please use a different email.");
                }

                // Create a new user
                var newUser = await _userService.CreateUserAsync(new UserValidationDTO
                {
                    GoogleId = googleId,
                    Email = email,
                    DisplayName = request.Username
                });

                return CreatedAtAction("GoogleSignIn", new { userId = newUser.Id },
                    $"User created successfully. Welcome, {newUser.DisplayName}!");
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
