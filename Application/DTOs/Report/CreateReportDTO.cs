namespace Application.DTOs.Report;

/// <summary>
///     DTO for creating a report on an entity (user, post, song, etc.)
/// </summary>
public class CreateReportDTO
{
    public required int EntityIdentifier { get; set; }
    public required int ReportableEntityTypeId { get; set; }
    public required int ReportReasonTypeId { get; set; }
}