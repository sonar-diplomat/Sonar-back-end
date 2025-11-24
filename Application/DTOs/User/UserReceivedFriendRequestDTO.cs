namespace Application.DTOs.User;

public class UserReceivedFriendRequestDTO
{
    public required int Id { get; set; }
    public required int FromUserId { get; set; }
    public required string FromUserName { get; set; }
    public required DateTime RequestedAt { get; set; }
}