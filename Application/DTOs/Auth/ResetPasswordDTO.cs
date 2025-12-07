namespace Application.DTOs.Auth;

public class ResetPasswordDTO
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}

