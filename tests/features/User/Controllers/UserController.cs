using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Features.User.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkWithUsers()
    {
        var users = new List<UserDTO>
        {
            new UserDTO { Id = Guid.NewGuid(), DisplayName = "User1" },
            new UserDTO { Id = Guid.NewGuid(), DisplayName = "User2" }
        };
        _userServiceMock.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);

        var result = await _userController.GetAllUsers();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDTO>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count());
    }

    [Fact]
    public async Task GetUser_ReturnsOkWithUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new UserDTO { Id = userId, DisplayName = "User1" };
        _userServiceMock.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(user);

        var result = await _userController.GetUser(userId.ToString());

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal(userId, returnedUser.Id);
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(service => service.GetUserAsync(userId)).ReturnsAsync((UserDTO)null);

        var result = await _userController.GetUser(userId.ToString());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetUserByGoogleId_ReturnsOkWithUser_WhenUserExists()
    {
        var googleId = "google123";
        var user = new UserDTO { Id = Guid.NewGuid(), DisplayName = "User1" };
        _userServiceMock.Setup(service => service.GetUserByGoogleIdAsync(googleId)).ReturnsAsync(user);

        var result = await _userController.GetUserByGoogleId(googleId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal(user.DisplayName, returnedUser.DisplayName);
    }

    [Fact]
    public async Task GetUserByGoogleId_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var googleId = "google123";
        _userServiceMock.Setup(service => service.GetUserByGoogleIdAsync(googleId)).ReturnsAsync((UserDTO)null);

        var result = await _userController.GetUserByGoogleId(googleId);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateUserGoldXp_ReturnsOk_WhenUpdateSuccessful()
    {
        var userId = Guid.NewGuid();
        var user = new UserDTO { Id = userId, Gold = 50, XP = 100 };
        _userServiceMock.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(user);
        _userServiceMock.Setup(service => service.UpdateUserGoldXp(userId, 10, 20)).ReturnsAsync(true);

        var result = await _userController.UpdateUserGoldXp(userId.ToString(), 10, 20);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }


    [Fact]
    public async Task AddSkinToUser_ReturnsOk_WhenSkinAddedSuccessfully()
    {
        var userId = Guid.NewGuid();
        var skinName = "CoolSkin";
        _userServiceMock.Setup(service => service.AddSkinToUserAsync(userId, skinName)).ReturnsAsync(true);

        var result = await _userController.AddSkinToUser(userId.ToString(), skinName);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddSkinToUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var skinName = "CoolSkin";
        _userServiceMock.Setup(service => service.AddSkinToUserAsync(userId, skinName)).ReturnsAsync(false);

        var result = await _userController.AddSkinToUser(userId.ToString(), skinName);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task EquipSkin_ReturnsOk_WhenSkinEquippedSuccessfully()
    {
        var userId = Guid.NewGuid();
        var skinName = "CoolSkin";
        _userServiceMock.Setup(service => service.EquipSkinAsync(userId, skinName)).ReturnsAsync(true);

        var result = await _userController.EquipSkin(userId.ToString(), skinName);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task EquipSkin_ReturnsNotFound_WhenSkinOrUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var skinName = "CoolSkin";
        _userServiceMock.Setup(service => service.EquipSkinAsync(userId, skinName)).ReturnsAsync(false);

        var result = await _userController.EquipSkin(userId.ToString(), skinName);

        Assert.IsType<NotFoundObjectResult>(result);
    }
    
}
