namespace Application.DTOs.Recommendations;

public class RecentTrackDTO
{
    public required string TrackId { get; set; }
    public DateTime LastPlayedAtUtc { get; set; }
    public string? ContextId { get; set; }
    public int ContextType { get; set; }
}


