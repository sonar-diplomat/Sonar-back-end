namespace Application.DTOs.User;

public class UserFollowDTO
{
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public int FollowingId { get; set; }
    public DateTime FollowedAt { get; set; }
}

public class UserFollowerDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PublicIdentifier { get; set; } = string.Empty;
    public int AvatarImageId { get; set; }
    public DateTime FollowedAt { get; set; }
}

public class UserFollowingDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PublicIdentifier { get; set; } = string.Empty;
    public int AvatarImageId { get; set; }
    public DateTime FollowedAt { get; set; }
}

