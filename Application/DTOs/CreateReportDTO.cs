namespace Application.DTOs;

/// <summary>
///     DTO for creating a report on an entity (user, post, song, etc.)
/// </summary>
public class CreateReportDTO
{
    public required int EntityIdentifier { get; set; }
    public required int ReportableEntityTypeId { get; set; }
    public required int ReporterId { get; set; }
    public required List<int> ReportReasonTypeIds { get; set; } = [];
}