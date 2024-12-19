using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Features.Sprite.Entities;
using Data;

public class SpriteControllerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly SpriteController _controller;

    public SpriteControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new AppDbContext(options);
        _mockContext = new Mock<AppDbContext>(options);
        _controller = new SpriteController(context);
    }

    [Fact]
    public async Task UploadSprite_ReturnsOk_WhenSpriteIsUploadedSuccessfully()
    {
        var fileMock = new Mock<IFormFile>();
        var content = "Test image content";
        var fileName = "test.png";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(ms.Length);

        var spriteName = "TestSprite";

        var result = await _controller.UploadSprite(fileMock.Object, spriteName);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Sprite uploaded successfully!", ((dynamic)okResult.Value).message);
    }

    [Fact]
    public async Task UploadSprite_ReturnsBadRequest_WhenNoFileIsUploaded()
    {
        var result = await _controller.UploadSprite(null, "TestSprite");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file uploaded.", badRequestResult.Value);
    }

// [Fact]
// public async Task GetSprite_ReturnsFile_WhenSpriteExists()
// {
//  
//     var options = new DbContextOptionsBuilder<AppDbContext>()
//         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//         .Options;

//     await using var context = new AppDbContext(options);

//     
//     var sprite = new SpriteEntity
//     {
//         Id = 1, // Integer ID
//         Name = "TestSprite",
//         ImageData = new byte[] { 1, 2, 3 }
//     };

//     await context.Sprites.AddAsync(sprite);
//     await context.SaveChangesAsync();

//     var controller = new SpriteController(context);

//     
//     var result = await controller.GetSprite(sprite.Id.ToString()); // Pass ID as a string

//     
//     var fileResult = Assert.IsType<FileContentResult>(result);
//     Assert.Equal("image/png", fileResult.ContentType);
//     Assert.Equal(sprite.ImageData, fileResult.FileContents);
// }


// [Fact]
// public async Task GetSprite_ReturnsNotFound_WhenSpriteDoesNotExist()
// {

//     var options = new DbContextOptionsBuilder<AppDbContext>()
//         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//         .Options;

//     await using var context = new AppDbContext(options);
//     var controller = new SpriteController(context);

//     
//     var nonExistentId = 999; 
//     var stringId = nonExistentId.ToString();

//     
//     var result = await controller.GetSprite(stringId);

//     
//     Assert.IsType<NotFoundResult>(result);
// }



    
[Fact]
public async Task GetSpriteByName_ReturnsFile_WhenSpriteExists()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    await using var context = new AppDbContext(options);

    var spriteName = "TestSprite";
    var sprite = new SpriteEntity
    {
        Id = 1, // Integer ID
        Name = spriteName,
        ImageData = new byte[] { 4, 5, 6 }
    };

    await context.Sprites.AddAsync(sprite);
    await context.SaveChangesAsync();

    var controller = new SpriteController(context);

    // Act
    var result = await controller.GetSpriteByName(spriteName);

    // Assert
    var fileResult = Assert.IsType<FileContentResult>(result);
    Assert.Equal("image/png", fileResult.ContentType);
    Assert.Equal(sprite.ImageData, fileResult.FileContents);
}


    [Fact]
    public async Task GetSpriteByName_ReturnsNotFound_WhenSpriteDoesNotExist()
    {
        // Act
        var result = await _controller.GetSpriteByName("NonExistentSprite");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
