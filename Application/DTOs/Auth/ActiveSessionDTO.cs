namespace Application.DTOs.Auth;

public class ActiveSessionDTO
{
    public int Id { get; set; }
    public string? DeviceName { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }
}