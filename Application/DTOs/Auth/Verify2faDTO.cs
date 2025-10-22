namespace Application.DTOs.Auth;

public class Verify2FaDTO
{
    public string Email { get; set; }
    public string Code { get; set; }
}