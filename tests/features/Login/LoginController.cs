// using Xunit;
// using Moq;
// using Microsoft.AspNetCore.Mvc;
// using Google.Apis.Auth;
// using System;
// using System.Threading.Tasks;
// using features.Login;
// using Features.User.DTOs;

// namespace Features.Login
// {
//     public class LoginControllerTests
//     {
//         private readonly Mock<IUserService> _mockUserService;

//         public LoginControllerTests()
//         {
//             _mockUserService = new Mock<IUserService>();

//             // Set up environment variable for Google Client ID
//             Environment.SetEnvironmentVariable("GOOGLE_API_CLIENT_ID", "fake-client-id");
//         }

//         // [Fact]
//         // public async Task GoogleSignIn_ReturnsOk_WhenTokenIsValid()
//         // {
//         //     // Arrange
//         //     var controller = new LoginController(_mockUserService.Object);

//         //     // This is a valid token for testing (use a test token or generate one manually)
//         //     var validToken = "valid-test-token";

//         //     _mockUserService
//         //         .Setup(s => s.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
//         //         .ReturnsAsync(new UserValidationDTO
//         //         {
//         //             GoogleId = "fake-google-id",
//         //             Email = "test@example.com",
//         //             DisplayName = "Test User"
//         //         });

//         //     var request = new GoogleSignInRequest
//         //     {
//         //         Token = validToken,
//         //         Username = "testuser"
//         //     };

//         //     // Act
//         //     var result = await controller.GoogleSignIn(request);

//         //     // Assert
//         //     var okResult = Assert.IsType<OkObjectResult>(result);
//         //     var response = okResult.Value as dynamic;
//         //     Assert.NotNull(response);
//         //     Assert.Contains("Welcome back", response.message.ToString());
//         // }

//         [Fact]
//         public async Task GoogleSignIn_ReturnsUnauthorized_WhenTokenIsInvalid()
//         {
//             // Arrange
//             var controller = new LoginController(_mockUserService.Object);

//             // This is an invalid token for testing
//             var invalidToken = "invalid-test-token";

//             var request = new GoogleSignInRequest
//             {
//                 Token = invalidToken,
//                 Username = "testuser"
//             };

//             // Act
//             var result = await controller.GoogleSignIn(request);

//             // Assert
//             var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
//             var response = unauthorizedResult.Value as dynamic;
//             Assert.NotNull(response);
//             //Assert.Contains("Invalid token", response.message.ToString());
//         }

//         [Fact]
//         public async Task GoogleSignIn_ReturnsInternalServerError_WhenUnexpectedErrorOccurs()
//         {
//             // Arrange
//             var controller = new LoginController(_mockUserService.Object);

//             var validToken = "valid-test-token";

//             _mockUserService
//                 .Setup(s => s.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
//                 .ThrowsAsync(new Exception("Unexpected error"));

//             var request = new GoogleSignInRequest
//             {
//                 Token = validToken,
//                 Username = "testuser"
//             };

//             // Act
//             var result = await controller.GoogleSignIn(request);

//             // Assert
//             //var errorResult = Assert.IsType<ObjectResult>(result);
//             Assert.Equal(500, errorResult.StatusCode);
//             var response = errorResult.Value as dynamic;
//             Assert.NotNull(response);
//             Assert.Contains("An error occurred", response.message.ToString());
//         }
//     }
// }
