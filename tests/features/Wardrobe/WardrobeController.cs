using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Features.Wardrobe.DTOs;
using Features.Wardrobe.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Features.User.DTOs;


public class WardrobeControllerTests
{
    private readonly Mock<IWardrobeService> _mockWardrobeService;
    private readonly WardrobeController _controller;

    public WardrobeControllerTests()
    {
        _mockWardrobeService = new Mock<IWardrobeService>();
        _controller = new WardrobeController(_mockWardrobeService.Object);
    }

    [Fact]
    public async Task GetAllItems_ReturnsOk_WithAllWardrobeItems()
    {
        // Arrange
        var items = new List<WardrobeItemDTO>
        {
            new WardrobeItemDTO { Id = Guid.NewGuid(), Name = "Item1", Price = 100, RankRequirement = Rank.Noob },
            new WardrobeItemDTO { Id = Guid.NewGuid(), Name = "Item2", Price = 200, RankRequirement = Rank.Pro }
        };

        _mockWardrobeService.Setup(service => service.GetAllWardrobeItemsAsync())
            .ReturnsAsync(items);

        // Act
        var result = await _controller.GetAllItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItems = Assert.IsType<List<WardrobeItemDTO>>(okResult.Value);
        Assert.Equal(items.Count, returnedItems.Count);
    }

    [Fact]
    public async Task CreateWardrobeItem_ReturnsBadRequest_WhenPriceIsNegative()
    {
        // Arrange
        var itemDto = new CreateWardrobeItemDTO
        {
            Name = "InvalidItem",
            Price = -100,
            RankRequirement = Rank.Noob
        };

        // Act
        var result = await _controller.CreateWardrobeItem(itemDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Price cannot be negative", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateWardrobeItem_ReturnsCreated_WhenItemIsCreatedSuccessfully()
    {
        // Arrange
        var itemDto = new CreateWardrobeItemDTO
        {
            Name = "ValidItem",
            Price = 150,
            RankRequirement = Rank.Noob
        };

        var createdItem = new WardrobeItemDTO
        {
            Id = Guid.NewGuid(),
            Name = "ValidItem",
            Price = 150,
            RankRequirement = Rank.Noob
        };

        _mockWardrobeService.Setup(service => service.CreateWardrobeItemAsync(itemDto))
            .ReturnsAsync(createdItem);

        // Act
        var result = await _controller.CreateWardrobeItem(itemDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedItem = Assert.IsType<WardrobeItemDTO>(createdResult.Value);
        Assert.Equal(createdItem.Name, returnedItem.Name);
        Assert.Equal(createdItem.Price, returnedItem.Price);
        Assert.Equal(createdItem.RankRequirement, returnedItem.RankRequirement);
    }

    [Fact]
    public async Task GetWardrobeItemByName_ReturnsOk_WhenItemExists()
    {
        // Arrange
        var itemName = "ExistingItem";
        var wardrobeItem = new WardrobeItemDTO
        {
            Id = Guid.NewGuid(),
            Name = itemName,
            Price = 300,
            RankRequirement = Rank.Pro
        };

        _mockWardrobeService.Setup(service => service.GetWardrobeItemByNameAsync(itemName))
            .ReturnsAsync(wardrobeItem);

        // Act
        var result = await _controller.GetWardrobeItemByName(itemName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItem = Assert.IsType<WardrobeItemDTO>(okResult.Value);
        Assert.Equal(itemName, returnedItem.Name);
    }

    [Fact]
    public async Task GetWardrobeItemByName_ReturnsNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        var itemName = "NonExistentItem";

        _mockWardrobeService.Setup(service => service.GetWardrobeItemByNameAsync(itemName))
            .ReturnsAsync((WardrobeItemDTO)null);

        // Act
        var result = await _controller.GetWardrobeItemByName(itemName);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Wardrobe item with name '{itemName}' not found", notFoundResult.Value);
    }
}
