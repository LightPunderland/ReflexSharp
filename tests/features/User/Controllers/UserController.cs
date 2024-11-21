using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Features.User.DTOs;
using Features.User.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkResult_WithListOfUsers()
    {
        
        var users = new List<UserDTO>
        {
            new UserDTO { Id = Guid.NewGuid(), DisplayName = "John", Email = "john@example.com" },
            new UserDTO { Id = Guid.NewGuid(), DisplayName = "Jane", Email = "jane@example.com" }
        };
        _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        
        var result = await _controller.GetAllUsers();

       
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDTO>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count());
    }

    [Fact]
    public async Task GetUser_ValidUserId_ReturnsOkResult_WithUser()
    {
      
        var userId = Guid.NewGuid();
        var user = new UserDTO { Id = userId, DisplayName = "John", Email = "john@example.com" };
        _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(user);

        
        var result = await _controller.GetUser(userId.ToString());

        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal(user.Id, returnedUser.Id);
    }

    [Fact]
    public async Task GetUser_InvalidUserId_ReturnsNotFound()
    {
        
        _mockUserService.Setup(s => s.GetUserAsync(It.IsAny<Guid>())).ReturnsAsync((UserDTO)null);

        
        var result = await _controller.GetUser("invalid-guid");

        
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetUserByGoogleId_ValidGoogleId_ReturnsOkResult_WithUser()
    {
       
        var googleId = "google123";
        var user = new UserDTO { Id = Guid.NewGuid(), DisplayName = "John", Email = "john@example.com" };
        _mockUserService.Setup(s => s.GetUserByGoogleIdAsync(googleId)).ReturnsAsync(user);

        
        var result = await _controller.GetUserByGoogleId(googleId);

        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal(user.Id, returnedUser.Id);
    }

    [Fact]
    public async Task GetUserByGoogleId_InvalidGoogleId_ReturnsNotFound()
    {
        
        _mockUserService.Setup(s => s.GetUserByGoogleIdAsync(It.IsAny<string>())).ReturnsAsync((UserDTO)null);

        
        var result = await _controller.GetUserByGoogleId("invalid-google-id");

       
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
public async Task UpdateUserGoldXp_ValidUserId_ReturnsOkResult_WithUpdatedGoldAndXp()
{
   
    var userId = Guid.NewGuid();
    var user = new User { Id = userId, Gold = 50, XP = 100 };
    _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(new UserDTO
    {
        Id = userId,
        Gold = user.Gold,
        XP = user.XP
    });

    _mockUserService.Setup(s => s.UpdateUserGoldXp(userId, 20, 30)).ReturnsAsync(true);

   
    var result = await _controller.UpdateUserGoldXp(userId.ToString(), 20, 30);

    
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    dynamic response = okResult.Value; 
    Assert.Contains("Gold 50 ->", response.ToString());
    Assert.Contains("XP 100 ->", response.ToString());
}


    [Fact]
    public async Task UpdateUserGoldXp_InvalidUserId_ReturnsNotFound()
    {
        
        _mockUserService.Setup(s => s.GetUserAsync(It.IsAny<Guid>())).ReturnsAsync((UserDTO)null);

        
        var result = await _controller.UpdateUserGoldXp("invalid-guid", 20, 30);

       
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
