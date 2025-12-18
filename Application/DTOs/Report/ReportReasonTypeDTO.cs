namespace Application.DTOs.Report;

public class ReportReasonTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TimeSpan RecommendedSuspensionDuration { get; set; }
    public List<int> ApplicableEntityTypeIds { get; set; } = new();
}

