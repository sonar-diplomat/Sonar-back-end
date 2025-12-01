namespace Application.DTOs.Search;

public class ArtistSearchItemDTO
{
    public required int Id { get; set; }
    public required string ArtistName { get; set; }
    public required int UserId { get; set; }
    public int? AvatarImageId { get; set; }
    public required int TrackCount { get; set; }
    public required int AlbumCount { get; set; }
}

