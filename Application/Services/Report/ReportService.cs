using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Report;
using Application.Extensions;
using Entities.Models.Report;
using Microsoft.EntityFrameworkCore;
using ReportModel = Entities.Models.Report.Report;

namespace Application.Services.Report;

public class ReportService(
    IReportRepository repository,
    IReportableEntityTypeService reportableEntityTypeService,
    IReportReasonTypeService reportReasonTypeService,
    IUserService userService
)
    : GenericService<ReportModel>(repository), IReportService
{
    public async Task<ReportModel> GetByIdValidatedFullAsync(int id)
    {
        return await repository
            .SnInclude(r => r.Reporter)
            .SnInclude(r => r.ReportableEntityType)
            .SnInclude(r => r.ReportReasonType)
            .GetByIdValidatedAsync(id);
    }

    public async Task<ReportModel> CreateReportAsync(int userId, CreateReportDTO dto)
    {
        ReportReasonType? reasonType = await reportReasonTypeService.GetByIdAsync(dto.ReportReasonTypeId);
        if (reasonType == null)
            throw new InvalidOperationException($"Report reason type with ID {dto.ReportReasonTypeId} not found");

        ReportModel report = new()
        {
            EntityIdentifier = dto.EntityIdentifier,
            ReportableEntityTypeId = dto.ReportableEntityTypeId,
            ReporterId = userId,
            ReportReasonTypeId = dto.ReportReasonTypeId,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow
        };

        return await repository.AddAsync(report);
    }

    public async Task CloseReportAsync(int reportId)
    {
        ReportModel? report = await GetByIdValidatedAsync(reportId);
        report.IsClosed = true;
        await repository.UpdateAsync(report);
    }

    public async Task<IEnumerable<ReportModel>> GetReportsFilteredAsync(ReportFilterDTO dto)
    {
        IQueryable<ReportModel> allReports = await repository.GetAllAsync();
        return await allReports
            .Where(r =>
        (!dto.entityId.HasValue || r.EntityIdentifier == dto.entityId) &&
        (!dto.typeId.HasValue || r.ReportableEntityTypeId == dto.typeId) &&
        (!dto.isClosed.HasValue || r.IsClosed == dto.isClosed) &&
        (!dto.reporterId.HasValue || r.ReporterId == dto.reporterId)
    )
            .ToListAsync();
    }

    public async Task<IEnumerable<ReportModel>> GetReportsByReporterAsync(int reporterId)
    {
        IQueryable<ReportModel> allReports = await repository.GetAllAsync();
        return await allReports
            .Where(r => r.ReporterId == reporterId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReportModel>> GetOpenReportsAsync()
    {
        IQueryable<ReportModel> allReports = await repository.GetAllAsync();
        return await allReports
            .Where(r => !r.IsClosed)
            .ToListAsync();
    }
}