namespace Application.DTOs.Report
{
    public record ReportFilterDTO(int? entityId, int? typeId, bool? isClosed, int? reporterId) { }
}
