namespace Application.DTOs.Auth;

public class ChangePasswordDTO
{
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
    public required string OldPassword { get; set; }
}