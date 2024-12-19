using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Features.Wardrobe.DTOs;
using Features.Wardrobe.Entities;
using Features.Wardrobe.Services;
using Features.User.DTOs;
using Data;

public class WardrobeServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<IUserService> _mockUserService;
    private readonly WardrobeService _service;

    public WardrobeServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);
        _mockUserService = new Mock<IUserService>();
        _service = new WardrobeService(_mockContext.Object, _mockUserService.Object);
    }

   [Fact]
public async Task GetAllWardrobeItemsAsync_ReturnsAllItems()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    await using var context = new AppDbContext(options);

    var wardrobeItems = new List<WardrobeItem>
    {
        new WardrobeItem { Id = Guid.NewGuid(), Name = "Item1", Price = 100, RequiredRank = Rank.Pro },
        new WardrobeItem { Id = Guid.NewGuid(), Name = "Item2", Price = 200, RequiredRank = Rank.Noob }
    };

    await context.WardrobeItems.AddRangeAsync(wardrobeItems);
    await context.SaveChangesAsync();

    var wardrobeService = new WardrobeService(context, new Mock<IUserService>().Object);

    // Act
    var result = await wardrobeService.GetAllWardrobeItemsAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(wardrobeItems.Count, result.Count());
    Assert.Equal("Item1", result.First().Name);
}


[Fact]
public async Task GetWardrobeItemAsync_ReturnsItem_WhenItemExists()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    await using var context = new AppDbContext(options);

    var itemId = Guid.NewGuid();
    var wardrobeItem = new WardrobeItem
    {
        Id = itemId,
        Name = "Item1",
        Price = 100,
        RequiredRank = Rank.Pro
    };

    await context.WardrobeItems.AddAsync(wardrobeItem);
    await context.SaveChangesAsync();

    var wardrobeService = new WardrobeService(context, new Mock<IUserService>().Object);

    // Act
    var result = await wardrobeService.GetWardrobeItemAsync(itemId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Item1", result?.Name);
    Assert.Equal(100, result?.Price);
    Assert.Equal(Rank.Pro, result?.RankRequirement);
}


    [Fact]
public async Task GetWardrobeItemAsync_ReturnsNull_WhenItemDoesNotExist()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure a unique in-memory database for the test
        .Options;

    await using var context = new AppDbContext(options);

    var wardrobeService = new WardrobeService(context, new Mock<IUserService>().Object);

    // Act
    var result = await wardrobeService.GetWardrobeItemAsync(Guid.NewGuid()); // Query for a non-existent item

    // Assert
    Assert.Null(result); // Ensure that the method returns null for a non-existent item
}


[Fact]
public async Task CreateWardrobeItemAsync_CreatesItemSuccessfully()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    await using var context = new AppDbContext(options);

    var createItemDto = new CreateWardrobeItemDTO
    {
        Name = "NewItem",
        Price = 150,
        RankRequirement = Rank.Noob
    };

    var wardrobeService = new WardrobeService(context, new Mock<IUserService>().Object);

    // Act
    var result = await wardrobeService.CreateWardrobeItemAsync(createItemDto);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(createItemDto.Name, result.Name);
    Assert.Equal(createItemDto.Price, result.Price);
    Assert.Equal(createItemDto.RankRequirement, result.RankRequirement);

    // Verify the item was saved in the database
    var savedItem = await context.WardrobeItems.FirstOrDefaultAsync(w => w.Name == createItemDto.Name);
    Assert.NotNull(savedItem);
    Assert.Equal(createItemDto.Name, savedItem?.Name);
    Assert.Equal(createItemDto.Price, savedItem?.Price);
    Assert.Equal(createItemDto.RankRequirement, savedItem?.RequiredRank);
}

[Fact]
public async Task GetWardrobeItemByNameAsync_ReturnsItem_WhenNameExists()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    await using var context = new AppDbContext(options);

    var wardrobeItem = new WardrobeItem
    {
        Id = Guid.NewGuid(),
        Name = "UniqueName",
        Price = 300,
        RequiredRank = Rank.Pro
    };

    await context.WardrobeItems.AddAsync(wardrobeItem);
    await context.SaveChangesAsync();

    var wardrobeService = new WardrobeService(context, new Mock<IUserService>().Object);

    // Act
    var result = await wardrobeService.GetWardrobeItemByNameAsync("UniqueName");

    // Assert
    Assert.NotNull(result);
    Assert.Equal("UniqueName", result?.Name);
    Assert.Equal(300, result?.Price);
    Assert.Equal(Rank.Pro, result?.RankRequirement);
}


    [Fact]
public async Task GetWardrobeItemByNameAsync_ReturnsNull_WhenNameDoesNotExist()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    await using var context = new AppDbContext(options);
    var wardrobeService = new WardrobeService(context, new Mock<IUserService>().Object);

    // Act
    var result = await wardrobeService.GetWardrobeItemByNameAsync("NonExistentName");

    // Assert
    Assert.Null(result);
}

}
