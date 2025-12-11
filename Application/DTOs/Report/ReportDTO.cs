using Application.DTOs.User;

namespace Application.DTOs.Report;

public class ReportDTO
{
    public int Id { get; set; }
    public bool IsClosed { get; set; }
    public int EntityIdentifier { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public UserResponseDTO Reporter { get; set; }
    public ReportableEntityTypeDTO EntityType { get; set; }
    public ReportReasonTypeDTO? ReportReasonType { get; set; }
}

