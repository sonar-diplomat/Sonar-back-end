namespace Application.DTOs.Music;

public class PlaylistResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Related data
    public int CoverId { get; set; }
    public string CreatorName { get; set; }
    public int TrackCount { get; set; }
    public List<string> ContributorNames { get; set; }
}

