using Application.DTOs.Access;
using System.Text.Json.Serialization;

namespace Application.DTOs.User;

public record UserProfileDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PublicIdentifier { get; set; }
    public string? Biography { get; set; }
    public int AvatarImageId { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public ICollection<AccessFeatureDTO> AccessFeatures { get; set; }
    public ICollection<UserPlaylistDTO> PublicPlaylists { get; set; }
}

