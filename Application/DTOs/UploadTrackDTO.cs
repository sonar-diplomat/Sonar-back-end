using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public class UploadTrackDTO
{
    public required string Title { get; set; }
    public required bool IsExplicit { get; set; }
    public required bool DrivingDisturbingNoises { get; set; }
    public required IFormFile AudioFile { get; set; }
    public required IFormFile CoverFile { get; set; }
}