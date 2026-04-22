using Xunit;
using Microsoft.AspNetCore.Http;
using System.IO;
using PROG7311_POE.Services;

namespace PROG7311_POE.Tests.Services
{
    public class FileValidatorTests
    {
        [Fact]
        public void IsValidPdf_ValidPdf_ReturnsTrue()
        {
            // Arrange
            var file = new FormFile(new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46 }), 0, 4, "test", "document.pdf");

            // Result
            bool result = FileValidator.IsValidPdf(file, out string error);

            // Assert
            Assert.True(result);
            Assert.Empty(error);
        }

        [Fact]
        public void IsValidPdf_ExeFile_ReturnsFalseWithError()
        {
            var file = new FormFile(new MemoryStream(), 0, 0, "bad", "virus.exe");
            bool result = FileValidator.IsValidPdf(file, out string error);
            Assert.False(result);
            Assert.Equal("Only PDF files are allowed.", error);
        }

        [Fact]
        public void IsValidPdf_NullFile_ReturnsFalse()
        {
            bool result = FileValidator.IsValidPdf(null, out string error);
            Assert.False(result);
            Assert.Equal("No file provided.", error);
        }

        [Fact]
        public void IsValidPdf_EmptyFile_ReturnsFalse()
        {
            var file = new FormFile(new MemoryStream(), 0, 0, "empty", "empty.pdf");
            bool result = FileValidator.IsValidPdf(file, out string error);
            Assert.False(result);
            Assert.Equal("File is empty.", error);
        }

        [Fact]
        public void IsValidPdf_NoExtension_ReturnsFalse()
        {
            var file = new FormFile(new MemoryStream(), 0, 100, "noext", "file");
            bool result = FileValidator.IsValidPdf(file, out string error);
            Assert.False(result);
            Assert.Equal("Only PDF files are allowed.", error);
        }
    }
}