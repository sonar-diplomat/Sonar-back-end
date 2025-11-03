namespace Application.DTOs.User;

public class UserResponseDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PublicIdentifier { get; set; }
    public string? Biography { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string AvatarUrl { get; set; }
}

