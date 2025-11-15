using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services.File;
using Application.Response;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.File;

[Route("api/blob")]
[ApiController]
public class ImageController(
    IImageFileService imageFileService,
    IImageFileRepository imageFileRepository,
    IFileStorageService fileStorageService) : ControllerBase
{

    /// <summary>
    /// Retrieves an image file by its ID.
    /// </summary>
    /// <param name="id">The ID of the image to retrieve.</param>
    /// <returns>Image file with appropriate content type.</returns>
    /// <response code="200">Image retrieved successfully.</response>
    /// <response code="404">Image not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImageById([FromRoute] int id)
    {
        var imageFile = await imageFileRepository.GetByIdAsync(id);

        if (imageFile == null)
        {
            throw ResponseFactory.Create<NotFoundResponse>([$"Image file with ID '{id}' not found"]);
        }

        byte[] imageBytes;
        try
        {
            imageBytes = await fileStorageService.GetFile(imageFile.Url);
        }
        catch
        {
            throw ResponseFactory.Create<NotFoundResponse>([$"Image file with ID '{id}' not found"]);
        }

        string contentType = GetContentType(imageFile.Url);

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