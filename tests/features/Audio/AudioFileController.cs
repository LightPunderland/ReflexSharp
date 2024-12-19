using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Features.Audio.Controllers;
using Features.Audio.Entities;
using Data;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class AudioControllerTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task UploadAudio_ReturnsBadRequest_WhenFileIsNull()
    {
        var context = CreateInMemoryContext();
        context.AudioFiles.RemoveRange(context.AudioFiles);
        var controller = new AudioController(context);

        var result = await controller.UploadAudio(null, "TestAudio");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No file uploaded.", badRequestResult.Value);
    }

    [Fact]
public async Task UploadAudio_ReturnsBadRequest_WhenNameIsEmpty()
{
    var context = CreateInMemoryContext();
    context.AudioFiles.RemoveRange(context.AudioFiles);
    var controller = new AudioController(context);

    var mockFile = new Mock<IFormFile>();
    var content = "Fake Audio Content";
    var fileName = "test.mp3";
    var memoryStream = new MemoryStream();
    var writer = new StreamWriter(memoryStream);
    writer.Write(content);
    writer.Flush();
    memoryStream.Position = 0;

    mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
    mockFile.Setup(f => f.FileName).Returns(fileName);
    mockFile.Setup(f => f.Length).Returns(memoryStream.Length);

    var result = await controller.UploadAudio(mockFile.Object, "");

    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Audio name is required.", badRequestResult.Value);
}


    [Fact]
public async Task UploadAudio_ReturnsOk_WhenFileIsValid()
{
    var context = CreateInMemoryContext();
    var controller = new AudioController(context);

    var mockFile = new Mock<IFormFile>();
    var content = "Fake Audio Content";
    var fileName = "test.mp3";
    var memoryStream = new MemoryStream();
    var writer = new StreamWriter(memoryStream);
    writer.Write(content);
    writer.Flush();
    memoryStream.Position = 0;

    mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
    mockFile.Setup(f => f.FileName).Returns(fileName);
    mockFile.Setup(f => f.Length).Returns(memoryStream.Length);

    var result = await controller.UploadAudio(mockFile.Object, "TestAudio");

    var okResult = Assert.IsType<OkObjectResult>(result);
    var responseValue = okResult.Value;

    var messageProperty = responseValue.GetType().GetProperty("message");
    var audioIdProperty = responseValue.GetType().GetProperty("audioId");

    Assert.NotNull(messageProperty);
    Assert.NotNull(audioIdProperty);

    var message = messageProperty.GetValue(responseValue)?.ToString();
    var audioId = audioIdProperty.GetValue(responseValue);

    Assert.Equal("Audio uploaded successfully!", message);
    Assert.NotNull(audioId); 
}


[Fact]
public async Task GetAudio_ReturnsNotFound_WhenAudioDoesNotExist()
{
    var context = CreateInMemoryContext();
    context.AudioFiles.RemoveRange(context.AudioFiles);
    context.SaveChanges();

    var controller = new AudioController(context);

    var result = await controller.GetAudio(1);

    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    var responseValue = notFoundResult.Value;

    var messageProperty = responseValue.GetType().GetProperty("message");
    Assert.NotNull(messageProperty);
    var message = messageProperty.GetValue(responseValue)?.ToString();

    Assert.Equal("Audio file not found.", message);
}




    [Fact]
    public async Task GetAudio_ReturnsFileStreamResult_WhenAudioExists()
    {
        var context = CreateInMemoryContext();
        context.AudioFiles.RemoveRange(context.AudioFiles);
        var audioFile = new AudioFileEntity
        {
            Id = 1,
            Name = "TestAudio",
            FileData = new byte[] { 1, 2, 3 }
        };
        context.AudioFiles.Add(audioFile);
        context.SaveChanges();

        var controller = new AudioController(context);

        var result = await controller.GetAudio(1);

        var fileStreamResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("audio/mpeg", fileStreamResult.ContentType);
        Assert.Equal("TestAudio.mp3", fileStreamResult.FileDownloadName);
    }

    [Fact]
    public async Task GetAllAudioFiles_ReturnsEmptyList_WhenNoAudioFilesExist()
    {
        var context = CreateInMemoryContext();
        var controller = new AudioController(context);

        var result = await controller.GetAllAudioFiles();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var audioList = Assert.IsType<List<object>>(okResult.Value);
        Assert.Empty(audioList);
    }

    [Fact]
public async Task GetAllAudioFiles_ReturnsAudioFiles_WhenAudioFilesExist()
{

    var context = CreateInMemoryContext();
    context.AudioFiles.RemoveRange(context.AudioFiles);
    context.SaveChanges();

    context.AudioFiles.AddRange(new List<AudioFileEntity>
    {
        new AudioFileEntity { Name = "Audio1", FileData = new byte[] { 1, 2 } },
        new AudioFileEntity { Name = "Audio2", FileData = new byte[] { 3, 4 } }
    });
    context.SaveChanges();

    var controller = new AudioController(context);

    var result = await controller.GetAllAudioFiles();

    var okResult = Assert.IsType<OkObjectResult>(result);
    var audioList = Assert.IsType<List<object>>(okResult.Value);
    Assert.Equal(2, audioList.Count);
}



    [Fact]
public async Task GetAudioFileSizeKB_ReturnsFileSize_WhenAudioExists()
{
    var context = CreateInMemoryContext();
    context.AudioFiles.RemoveRange(context.AudioFiles);
    var audioFile = new AudioFileEntity
    {
        Id = 1,
        Name = "TestAudio",
        FileData = new byte[2048] 
    };
    context.AudioFiles.Add(audioFile);
    context.SaveChanges();

    var controller = new AudioController(context);

    var result = await controller.GetAudioFileSizeKB(1);


    var okResult = Assert.IsType<OkObjectResult>(result);
    var responseValue = okResult.Value;

    var fileSizeProperty = responseValue.GetType().GetProperty("fileSize");
    Assert.NotNull(fileSizeProperty);
    var fileSize = fileSizeProperty.GetValue(responseValue)?.ToString();

    Assert.Equal("2.00 KB", fileSize);
}

}
