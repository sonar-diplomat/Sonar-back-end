namespace Application.DTOs.Auth;

public class ConfirmPasswordChangeDTO
{
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
    public required string OldPassword { get; set; }
}