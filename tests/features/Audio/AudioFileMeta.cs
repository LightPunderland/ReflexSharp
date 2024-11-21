using Xunit;
using Features.Audio.Entities; 
using Features.Audio.Extension; 

namespace Features.Audio.Tests 
{
    public class AudioFileExtensionsTests
    {
        [Fact]
        public void GetAudioFileSize_ReturnsCorrectSize_ForValidAudioFile()
        {
            
            var audioFile = new AudioFileEntity
            {
                FileData = new byte[2048] // 2048 bytes = 2 KB
            };

            
            var size = audioFile.GetAudioFileSize();

            
            Assert.Equal(2.0, size); 
        }

        [Fact]
        public void GetAudioFileSize_ReturnsZero_ForEmptyFileData()
        {
            
            var audioFile = new AudioFileEntity
            {
                FileData = new byte[0] 
            };

            
            var size = audioFile.GetAudioFileSize();

            
            Assert.Equal(0.0, size); 
        }

        [Fact]
        public void GetAudioFileSize_HandlesLargeFiles_Correctly()
        {
            
            var audioFile = new AudioFileEntity
            {
                FileData = new byte[10 * 1024 * 1024] // 10 MB = 10240 KB
            };

            
            var size = audioFile.GetAudioFileSize();

            
            Assert.Equal(10240.0, size); 
        }
    }
}
