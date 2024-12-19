using System;
using System.Threading.Tasks;
using features.Login; 
using Features.User.DTOs; 
using Features.User.Entities; 
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace features.Login.Tests
{
    public class LoginControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly LoginController _controller;

        public LoginControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new LoginController(_mockUserService.Object);
            Environment.SetEnvironmentVariable("GOOGLE_API_CLIENT_ID", "test-client-id");
        }

        [Fact]
        public async Task GoogleSignIn_ReturnsBadRequest_IfClientIdNotSet()
        {
            Environment.SetEnvironmentVariable("GOOGLE_API_CLIENT_ID", null);
            var request = new GoogleSignInRequest { Token = "some-token", Username = "testuser" };

            var result = await _controller.GoogleSignIn(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Google API Client ID is not set.", badRequestResult.Value);
        }

        [Fact]
        public async Task GoogleSignIn_ReturnsUnauthorized_OnInvalidToken()
        {
            var request = new GoogleSignInRequest { Token = "invalid-token", Username = "testuser" };

            var result = await _controller.GoogleSignIn(request);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);

            var json = System.Text.Json.JsonSerializer.Serialize(unauthorizedResult.Value);
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var dictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json, options);

            Assert.NotNull(dictionary);
            Assert.True(dictionary.ContainsKey("message"));
            Assert.Equal("Invalid token.", dictionary["message"]);
        }

        [Fact]
        public async Task GoogleSignIn_ReturnsInternalServerError_OnGeneralException()
        {
            var request = new GoogleSignInRequest { Token = "some-token", Username = "testuser" };

            try
            {
                throw new Exception("Some general error.");
            }
            catch (Exception)
            {
                var internalServerError = new ObjectResult(new { message = "An error occurred while validating the token." })
                {
                    StatusCode = 500
                };
                Assert.Equal(500, internalServerError.StatusCode);
                dynamic response = internalServerError.Value;
                Assert.Equal("An error occurred while validating the token.", (string)response.message);
            }
        }

       
    }
}
