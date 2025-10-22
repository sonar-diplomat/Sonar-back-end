namespace Application.DTOs;

public class DistributorAccountChangePasswordDTO
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}