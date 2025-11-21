using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Music;

public class CreatePlaylistDTO
{
    public required string Name { get; set; }
    public required IFormFile? Cover { get; set; }
}