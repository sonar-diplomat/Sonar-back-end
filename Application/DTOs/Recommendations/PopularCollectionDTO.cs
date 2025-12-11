namespace Application.DTOs.Recommendations;

public class PopularCollectionDTO
{
    public int CollectionId { get; set; }
    public int CollectionType { get; set; }
    public double Score { get; set; }
    public long Plays { get; set; }
    public long Likes { get; set; }
    public long Adds { get; set; }
}


