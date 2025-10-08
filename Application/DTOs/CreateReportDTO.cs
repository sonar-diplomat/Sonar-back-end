namespace Application.DTOs;

/// <summary>
///     DTO for creating a report on an entity (user, post, song, etc.)
/// </summary>
public class CreateReportDTO
{
    public int EntityIdentifier { get; set; }
    public int ReportableEntityTypeId { get; set; }
    public int ReporterId { get; set; }
    public List<int> ReportReasonTypeIds { get; set; } = new();
}
