using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Features.User.DTOs;
using Features.User.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UserServiceTests
{
    private readonly DbContextOptions<AppDbContext> _options;

    public UserServiceTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
       
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), GoogleId = "google123", Email = "user1@example.com", DisplayName = "User1", Rank = Rank.Noob },
            new User { Id = Guid.NewGuid(), GoogleId = "google456", Email = "user2@example.com", DisplayName = "User2", Rank = Rank.Pro }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        
        var result = await service.GetAllUsersAsync();

        
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserAsync_ValidUserId_ReturnsUser()
    {
       
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, GoogleId = "google123", Email = "user@example.com", DisplayName = "User", Rank = Rank.Noob };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        
        var result = await service.GetUserAsync(userId);

        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetUserAsync_InvalidUserId_ReturnsNull()
    {
        
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        
        var result = await service.GetUserAsync(Guid.NewGuid());

        
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserGoldXp_ValidUser_UpdatesGoldAndXp()
    {
        
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, GoogleId = "google123", Email = "user@example.com", DisplayName = "Tomas", Gold = 100, XP = 50 };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        
        var result = await service.UpdateUserGoldXp(userId, 50, 25);

        
        Assert.True(result);

        var updatedUser = await context.Users.FindAsync(userId);
        Assert.Equal(150, updatedUser.Gold);
        Assert.Equal(75, updatedUser.XP);
    }

    [Fact]
    public async Task UpdateUserGoldXp_InvalidUser_ReturnsFalse()
    {
        
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        
        var result = await service.UpdateUserGoldXp(Guid.NewGuid(), 50, 25);

        
        Assert.False(result);
    }

    // [Fact]
    // public async Task CheckUsernameAvailabilityAsync_UsernameAvailable_ReturnsTrue()
    // {
    //     
    //     using var context = new AppDbContext(_options);
    //     var service = new UserService(context);

    //     var user = new User { Id = Guid.NewGuid(), GoogleId = "google123", Email = "user1@example.com", DisplayName = "ExistingUser", Rank = Rank.Noob };
    //     context.Users.Add(user);
    //     await context.SaveChangesAsync();

    //     
    //     var result = await service.CheckUsernameAvailabilityAsync("NewUser");

    //     
    //     Assert.True(result);
    // }

    // [Fact]
    // public async Task CheckUsernameAvailabilityAsync_UsernameTaken_ReturnsFalse()
    // {
    //     
    //     using var context = new AppDbContext(_options);
    //     var service = new UserService(context);

    //     var user = new User { Id = Guid.NewGuid(), GoogleId = "google123", Email = "user1@example.com", DisplayName = "ExistingUser", Rank = Rank.Noob };
    //     context.Users.Add(user);
    //     await context.SaveChangesAsync();

    //     
    //     var result = await service.CheckUsernameAvailabilityAsync("ExistingUser");

    //     
    //     Assert.False(result);
    // }

    [Fact]
    public async Task GetUserByGoogleIdAsync_ValidGoogleId_ReturnsUser()
    {
        
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        var user = new User { Id = Guid.NewGuid(), GoogleId = "google123",  Email = "user1@example.com", DisplayName = "ExistingUser", Rank = Rank.Noob };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        
        var result = await service.GetUserByGoogleIdAsync("google123");

        
        Assert.NotNull(result);
        Assert.Equal(user.GoogleId, result.GoogleId);
    }

    [Fact]
    public async Task GetUserByGoogleIdAsync_InvalidGoogleId_ReturnsNull()
    {
       
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        
        var result = await service.GetUserByGoogleIdAsync("invalid-google-id");

        
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_ValidUserDetails_CreatesUser()
    {
       
        using var context = new AppDbContext(_options);
        var service = new UserService(context);

        var newUser = new UserValidationDTO
        {
            GoogleId = "google123",
            Email = "user@example.com",
            DisplayName = "NewUser"
        };

        
        var result = await service.CreateUserAsync(newUser);

        
        Assert.NotNull(result);
        Assert.Equal(newUser.GoogleId, result.GoogleId);
        Assert.Equal(newUser.Email, result.Email);
        Assert.Equal(newUser.DisplayName, result.DisplayName);
    }
}
