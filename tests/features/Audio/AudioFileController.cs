using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Features.Audio.Controllers;
using Features.Audio.Entities;
using Features.Audio.Exceptions; 
using Data;

namespace Features.Audio.Tests
{
    public class AudioControllerTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IAudioServices> _audioServicesMock;
        private readonly AudioController _controller;

        public AudioControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _audioServicesMock = new Mock<IAudioServices>();

            
            _controller = new AudioController(_context, _audioServicesMock.Object);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var audioFiles = new List<AudioFileEntity>
            {
                new AudioFileEntity { Id = 1, Name = "TestAudio1", FileData = new byte[] { 0x01, 0x02, 0x03 } },
                new AudioFileEntity { Id = 2, Name = "TestAudio2", FileData = new byte[] { 0x04, 0x05, 0x06 } }
            };

            _context.AudioFiles.AddRange(audioFiles);
            _context.SaveChanges();
        }

       [Fact]
public async Task UploadAudio_ReturnsOk_WhenValidInput()
{
   
    var fileMock = new Mock<IFormFile>();
    var content = "Fake audio file content";
    var memoryStream = new MemoryStream();
    var writer = new StreamWriter(memoryStream);
    writer.Write(content);
    writer.Flush();
    memoryStream.Position = 0;

    fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);
    fileMock.Setup(f => f.FileName).Returns("TestAudio.mp3");
    fileMock.Setup(f => f.Length).Returns(memoryStream.Length);

    
    _audioServicesMock.Setup(s => s.ValidateAudioFile(It.IsAny<IFormFile>()))
        .ReturnsAsync((true, string.Empty));

    
    var result = await _controller.UploadAudio(fileMock.Object, "TestAudio");

    
    var okResult = Assert.IsType<OkObjectResult>(result); 
    var responseJson = System.Text.Json.JsonSerializer.Serialize(okResult.Value); 
    var responseDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson); 

    Assert.NotNull(responseDict); 
    Assert.True(responseDict.ContainsKey("message")); 
    Assert.Equal("Audio uploaded successfully!", responseDict["message"].ToString());
    Assert.True(responseDict.ContainsKey("audioId")); 
    Assert.NotNull(responseDict["audioId"]); 
}


        [Fact]
public async Task UploadAudio_ReturnsBadRequest_WhenFileIsInvalid()
{
   
    var fileMock = new Mock<IFormFile>();
    var content = "Fake audio file content";
    var memoryStream = new MemoryStream();
    var writer = new StreamWriter(memoryStream);
    writer.Write(content);
    writer.Flush();
    memoryStream.Position = 0;

    fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);
    fileMock.Setup(f => f.FileName).Returns("InvalidAudio.mp3");
    fileMock.Setup(f => f.Length).Returns(memoryStream.Length);

    
    _audioServicesMock.Setup(s => s.ValidateAudioFile(It.IsAny<IFormFile>()))
        .ReturnsAsync((false, "Invalid audio format"));

    
    var result = await _controller.UploadAudio(fileMock.Object, "TestAudio");

    
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); 
    var responseJson = System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value); 
    var responseDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson); 

    Assert.NotNull(responseDict); 
    Assert.True(responseDict.ContainsKey("message")); 
    Assert.Equal("Invalid audio format", responseDict["message"].ToString()); 
}


        [Fact]
        public async Task GetAudio_ReturnsFile_WhenAudioExists()
        {
            
            var result = await _controller.GetAudio(1);

            
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("audio/mpeg", fileResult.ContentType);
            Assert.Equal("TestAudio1.mp3", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task GetAudio_ReturnsNotFound_WhenAudioDoesNotExist()
        {
            
            var result = await _controller.GetAudio(999);

            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Audio file not found.", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetAllAudioFiles_ReturnsListOfAudioFiles()
        {
            
            var result = await _controller.GetAllAudioFiles();

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var audioList = Assert.IsAssignableFrom<List<object>>(okResult.Value);
            Assert.Equal(2, audioList.Count);
        }

        [Fact]
        public async Task GetAudioFileSizeKB_ReturnsCorrectSize()
        {
            
            var result = await _controller.GetAudioFileSizeKB(1);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetAudioFileSizeKB_ReturnsNotFound_WhenAudioDoesNotExist()
        {
            
            var result = await _controller.GetAudioFileSizeKB(999);

           
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("No such audio file exists", notFoundResult.Value.ToString());
        }
    }
}
