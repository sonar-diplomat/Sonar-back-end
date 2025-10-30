namespace Application.DTOs.Music;

public class TrackDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int DurationInSeconds { get; set; }
    public required string CoverUrl { get; set; }
    public required string FileUrl { get; set; }
    public required IEnumerable<string> Artists { get; set; }
}