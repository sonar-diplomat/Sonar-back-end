namespace Application.DTOs.Search;

public class UserSearchItemDTO
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
    public required string PublicIdentifier { get; set; }
    public required int AvatarImageId { get; set; }
    public required bool IsArtist { get; set; }
    public string? ArtistName { get; set; }
}

