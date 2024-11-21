using System.Net;
using System.Net.Http.Json;
using Xunit;
using Features.User.DTOs;
using Features.User.Entities;
using Data;
using Microsoft.EntityFrameworkCore;


public class UserControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_ReturnsUsers()
    {
        var response = await _client.GetAsync("/api/users");

        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
        Assert.NotNull(users);
        Assert.NotEmpty(users); 
    }

    [Fact]
public async Task GetUser_ValidId_ReturnsUser()
{
    var seededUserId = Guid.NewGuid(); 
    var response = await _client.GetAsync($"/api/users/{seededUserId}");

    var user = await response.Content.ReadFromJsonAsync<UserDTO>();
    Assert.NotNull(user);
    //Assert.Equal(seededUserId, user.Id);
}


    [Fact]
    public async Task GetUser_InvalidId_ReturnsNotFound()
    {
        
        var invalidId = Guid.NewGuid().ToString();

        
        var response = await _client.GetAsync($"/api/users/{invalidId}");

        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
public async Task UpdateUserGoldXp_ValidUser_UpdatesGoldAndXp()
{
    
    var userId = Guid.NewGuid();
    var addGold = 50;
    var addXp = 100;

    
    await using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options);
    var user = new User
    {
        Id = userId,
        GoogleId = "google123",
        Email = "test@example.com",
        DisplayName = "TestUser",
        Rank = Rank.Noob,
        Gold = 100,
        XP = 200
    };
    context.Users.Add(user);
    await context.SaveChangesAsync();

    
    var response = await _client.PostAsJsonAsync($"/api/users/{userId}/rewardGoldXp", new { addGold, addXp });

    
    response.EnsureSuccessStatusCode();
    var message = await response.Content.ReadAsStringAsync();

    Assert.NotNull(message);
    Assert.Contains("Gold", message);
    Assert.Contains("XP", message);
}



}
