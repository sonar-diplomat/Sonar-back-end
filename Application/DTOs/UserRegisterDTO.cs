namespace Application.DTOs;

public class UserRegisterDTO
{
    public string UserName { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Locale { get; set; }
}
