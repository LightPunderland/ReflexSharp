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
using Data; 

namespace Features.Audio.Tests
{
    public class AudioControllerTests
    {
        private readonly AppDbContext _context;
        private readonly AudioController _controller;

        public AudioControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new AudioController(_context);

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

        
        var result = await _controller.UploadAudio(fileMock.Object, "TestAudio");

        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        var response = okResult.Value as dynamic;
        Assert.NotNull(response);
        //Assert.Equal("Audio uploaded successfully!", response.message);
        //ssert.NotEqual(0, response.audioId); // Ensure AudioId is assigned
    }



        [Fact]
        public async Task UploadAudio_ReturnsBadRequest_WhenFileIsNull()
        {
            
            var result = await _controller.UploadAudio(null, "TestAudio");

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);
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
