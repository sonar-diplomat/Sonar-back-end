using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Music;

public class UploadAlbumDTO
{
    public required string Name { get; set; }
    public required IFormFile Cover { get; set; }

    public required IEnumerable<string> Authors { get; set; }
}