namespace Application.DTOs.Auth;

public class ConfirmPasswordChangeDTO
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string OldPassword { get; set; }
}
