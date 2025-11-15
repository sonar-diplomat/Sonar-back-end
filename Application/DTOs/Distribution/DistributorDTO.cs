namespace Application.DTOs.Distribution;

public class DistributorDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public string ContactEmail { get; set; }
    public int CoverId { get; set; }
    public int AlbumCount { get; set; }
    public LicenseDTO License { get; set; }
}

