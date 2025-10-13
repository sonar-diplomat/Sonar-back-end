namespace Application.DTOs;

public class ChangePasswordDTO
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string OldPassword { get; set; }
}
