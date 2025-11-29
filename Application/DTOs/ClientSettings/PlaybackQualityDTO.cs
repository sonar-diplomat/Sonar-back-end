namespace Application.DTOs.ClientSettings;

public class PlaybackQualityDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int BitRate { get; set; }
    public required string Description { get; set; }
}

