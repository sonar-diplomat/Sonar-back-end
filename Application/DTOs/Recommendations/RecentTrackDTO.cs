namespace Application.DTOs.Recommendations;

public class RecentTrackDTO
{
    public int TrackId { get; set; }
    public DateTime LastPlayedAtUtc { get; set; }
    public int? ContextId { get; set; }
    public int ContextType { get; set; }
}


