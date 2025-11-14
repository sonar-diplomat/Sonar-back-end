namespace Application.DTOs.Distribution;

public class DistributorAccountDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool IsMaster { get; set; }
    public int DistributorId { get; set; }
}

