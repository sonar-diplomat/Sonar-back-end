namespace Application.DTOs.Music;

public class AlbumResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Related data
    public int CoverId { get; set; }
    public string DistributorName { get; set; }
    public int TrackCount { get; set; }
}

