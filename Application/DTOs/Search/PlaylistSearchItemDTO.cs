namespace Application.DTOs.Search;

public class PlaylistSearchItemDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int CoverId { get; set; }
    public required int TrackCount { get; set; }
    public required string CreatorName { get; set; }
    public required List<string> ContributorNames { get; set; }
}

