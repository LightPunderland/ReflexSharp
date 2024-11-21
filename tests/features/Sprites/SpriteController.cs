using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var context = new AppDbContext(options);
        _mockContext = new Mock<AppDbContext>(MockBehavior.Loose, options);
        _controller = new SpriteController(context);
    }

    
[Fact]
public async Task UploadSprite_ValidFile_ReturnsOk()
{
    
    var fileMock = new Mock<IFormFile>();
    var content = "Test Image Content";
    var fileName = "test.png";
    var name = "TestSprite";

    var ms = new MemoryStream();
    var writer = new StreamWriter(ms);
    writer.Write(content);
    writer.Flush();
    ms.Position = 0;

    fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
    fileMock.Setup(f => f.FileName).Returns(fileName);
    fileMock.Setup(f => f.Length).Returns(ms.Length);

    
    var result = await _controller.UploadSprite(fileMock.Object, name);

    
    var okResult = Assert.IsType<OkObjectResult>(result);

    
    var response = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
    var responseObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(response);

    
    Assert.NotNull(responseObj);
    Assert.True(responseObj.ContainsKey("message"));
    Assert.Equal("Sprite uploaded successfully!", responseObj["message"]);
}






    [Fact]
    public async Task UploadSprite_InvalidFile_ReturnsBadRequest()
    {
        
        IFormFile file = null;
        var name = "TestSprite";

        
        var result = await _controller.UploadSprite(file, name);

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file uploaded.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetSpriteByName_ValidName_ReturnsFile()
    {
        
        var name = "TestSprite";
        var sprite = new SpriteEntity { Name = name, ImageData = new byte[] { 1, 2, 3 } };

        await _controller.UploadSprite(
            new FormFile(new MemoryStream(sprite.ImageData), 0, sprite.ImageData.Length, "data", "image.png"),
            sprite.Name
        );

       
        var result = await _controller.GetSpriteByName(name);

        
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.Equal(sprite.ImageData, fileResult.FileContents);
    }

    [Fact]
    public async Task GetSpriteByName_InvalidName_ReturnsNotFound()
    {
        
        var name = "NonExistent";

        
        var result = await _controller.GetSpriteByName(name);

        
        Assert.IsType<NotFoundResult>(result);
    }
}
