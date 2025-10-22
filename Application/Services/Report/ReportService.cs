using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Response;
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
    public async Task<ReportModel> CreateReportAsync(CreateReportDTO dto)
    {
        List<ReportReasonType?> reasonTypes = [];
        foreach (int reasonTypeId in dto.ReportReasonTypeIds)
        {
            ReportReasonType reasonType = await reportReasonTypeService.GetByIdAsync(reasonTypeId);
            reasonTypes.Add(reasonType);
        }

        if (reasonTypes.Any(rt => rt == null))
            throw ResponseFactory.Create<BadRequestResponse>();

        ReportModel report = new()
        {
            EntityIdentifier = dto.EntityIdentifier,
            ReportableEntityType = await reportableEntityTypeService.GetByIdValidatedAsync(dto.ReportableEntityTypeId),
            Reporter = await userService.GetByIdValidatedAsync(dto.ReporterId),
            IsClosed = false,
            ReportReasonType = reasonTypes!
        };

        return await repository.AddAsync(report);
    }

    public async Task CloseReportAsync(int reportId)
    {
        ReportModel? report = await GetByIdValidatedAsync(reportId);
        report.IsClosed = true;
        await repository.UpdateAsync(report);
    }

    public async Task<IEnumerable<ReportModel>> GetReportsByEntityAsync(int entityId, int entityTypeId)
    {
        IQueryable<ReportModel> allReports = await repository.GetAllAsync();
        return await allReports
            .Where(r => r.EntityIdentifier == entityId && r.ReportableEntityTypeId == entityTypeId)
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