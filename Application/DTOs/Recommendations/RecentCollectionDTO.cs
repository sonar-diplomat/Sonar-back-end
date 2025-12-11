namespace Application.DTOs.Recommendations;

public class RecentCollectionDTO
{
    public required string CollectionId { get; set; }
    public int CollectionType { get; set; }
    public DateTime LastPlayedAtUtc { get; set; }
}


