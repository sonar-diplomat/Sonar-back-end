namespace Application.DTOs.User;

public class TopArtistDTO
{
    public required int Id { get; set; }
    public required string ArtistName { get; set; }
    public required int UserId { get; set; }
    public int? AvatarImageId { get; set; }
    public long PlayCount { get; set; }
}

