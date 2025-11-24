namespace Application.DTOs.User;

public class UserResponseDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Login { get; set; }
    public string PublicIdentifier { get; set; }
    public string? Biography { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int AvailableCurrency { get; set; }
    public int AvatarImageId { get; set; }
}

