namespace Application.DTOs.User;

public class UserUpdateDTO
{
    public string? PublicIdentifier { get; set; }
    public string? Biography { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
}
