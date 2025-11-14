using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Entities.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.File;

[Route("blob")]
[ApiController]
public class ImageController(
    IImageFileService imageFileService,
    IFileStorageService fileStorageService) : ControllerBase
{

    /// <summary>
    /// Retrieves an image file by its URL path.
    /// </summary>
    /// <param name="url">The URL path of the image to retrieve.</param>
    /// <returns>Image file with appropriate content type.</returns>
    /// <response code="200">Image retrieved successfully.</response>
    /// <response code="404">Image not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImageByUrl([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw ResponseFactory.Create<BadRequestResponse>(["URL parameter is required"]);
        }

        byte[] imageBytes;
        try
        {
            imageBytes = await fileStorageService.GetFile(url);
        }
        catch
        {
            throw ResponseFactory.Create<NotFoundResponse>([$"Image file with URL '{url}' not found"]);
        }

        string contentType = GetContentType(url);
        
        return File(imageBytes, contentType);
    }

    private static string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            _ => "image/jpeg" // Default to JPEG if unknown
        };
    }
}