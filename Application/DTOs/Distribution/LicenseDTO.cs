namespace Application.DTOs.Distribution;

public class LicenseDTO
{
    public int Id { get; set; }
    public DateTime IssuingDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int IssuerId { get; set; }
}

