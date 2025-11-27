using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public class UserRegisterDTO
{
    [Required(ErrorMessage = "Username cannot be empty")]
    public string UserName { get; set; } = null!;
    
    [Required(ErrorMessage = "Login cannot be empty")]
    public string Login { get; set; } = null!;
    
    [Required(ErrorMessage = "Email cannot be empty")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Password cannot be empty")]
    public string Password { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    [Required]
    public DateOnly DateOfBirth { get; set; }
    
    public string Locale { get; set; } = null!;
}
