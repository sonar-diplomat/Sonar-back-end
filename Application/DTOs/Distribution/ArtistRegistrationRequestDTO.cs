namespace Application.DTOs.Distribution;

public class ArtistRegistrationRequestDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ArtistName { get; set; }
    public int DistributorId { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool IsResolved => ResolvedAt.HasValue;
}

