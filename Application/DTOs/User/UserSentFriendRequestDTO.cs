namespace Application.DTOs.User;

public class UserSentFriendRequestDTO
{
    public required int Id { get; set; }
    public required int ToUserId { get; set; }
    public required string ToUserName { get; set; }
    public required DateTime RequestedAt { get; set; }
}