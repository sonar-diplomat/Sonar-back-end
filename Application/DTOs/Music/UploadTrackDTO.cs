using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Music;

public class UploadTrackDTO
{
    public required string Title { get; set; }
    public required bool IsExplicit { get; set; }
    public required bool DrivingDisturbingNoises { get; set; }
    public required IFormFile LowQualityAudioFile { get; set; }
    public required IFormFile? MediumQualityAudioFile { get; set; }
    public required IFormFile? HighQualityAudioFile { get; set; }
}