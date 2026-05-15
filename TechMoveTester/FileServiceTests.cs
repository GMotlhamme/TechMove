using Microsoft.AspNetCore.Http;
using Moq;
using TechMove.Services;

namespace TechMoveTester;


public class FileServiceTests
{
    [Fact]
    public void ValidateFile_ShouldThrowException_ForInvalidFileType()
    {
        // Arrange
        var service = new FileUploadValidationService();

        var fileName = "virus.exe";

        // Act & Assert
        Assert.Throws<Exception>(() =>
             service.ValidatePdf(fileName));
    }
}
