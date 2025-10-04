namespace Application.DTOs;

public class UserRegisterDTO
{
    public string Username { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Locale { get; set; }
    // TODO: add info for UserSession
}