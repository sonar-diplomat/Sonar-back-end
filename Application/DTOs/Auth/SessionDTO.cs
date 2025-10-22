namespace Application.DTOs;

public class SessionDTO
{
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceName { get; set; }
    public int DistributorId { get; set; }
}