using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Music;

public class UpdateTrackFileDTO
{
    public required int PlaybackQualityId { get; set; }
    public required IFormFile File { get; set; }
}