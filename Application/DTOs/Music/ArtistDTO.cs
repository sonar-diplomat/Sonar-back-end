namespace Application.DTOs.Music;

public class ArtistDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string ArtistName { get; set; }
    public UserBasicDTO? User { get; set; }
}

public class UserBasicDTO
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Login { get; set; }
    public required string PublicIdentifier { get; set; }
    public string? PhoneNumber { get; set; }
    public int? AvatarId { get; set; }
}

