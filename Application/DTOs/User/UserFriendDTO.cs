namespace Application.DTOs.User;

public class UserFriendDTO
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
    public required string PublicIdentifier { get; set; }
    public required int AvatarImageId { get; set; }
}