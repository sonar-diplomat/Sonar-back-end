using Application.Abstractions.Interfaces.Repository.Report;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Entities.Models.Report;
using Microsoft.EntityFrameworkCore;
using ReportModel = Entities.Models.Report.Report;

namespace Application.Services.Report;

public class ReportService(
    IReportRepository repository,
    IReportReasonTypeRepository reportReasonTypeRepository,
    IReportableEntityTypeRepository reportableEntityTypeRepository,
    AppExceptionFactory appExceptionFactory)
    : GenericService<ReportModel>(repository), IReportService
{
    public async Task<ReportModel> CreateReportAsync(CreateReportDTO dto)
    {
        // Валидация типа сущности
        ReportableEntityType? entityType = await reportableEntityTypeRepository.GetByIdAsync(dto.ReportableEntityTypeId);
        if (entityType == null)
            throw new NotImplementedException();
        
        // Валидация типа причина
        List<ReportReasonType> reasonTypes = new();
        foreach (int reasonTypeId in dto.ReportReasonTypeIds)
        {
            ReportReasonType? reasonType = await reportReasonTypeRepository.GetByIdAsync(reasonTypeId);
            if (reasonType == null)
                throw new NotImplementedException();
            reasonTypes.Add(reasonType);
        }
        
        ReportModel report = new()
        {
            EntityIdentifier = dto.EntityIdentifier,
            ReportableEntityTypeId = dto.ReportableEntityTypeId,
            ReporterId = dto.ReporterId,
            IsClosed = false,
            ReportReasonType = reasonTypes
        };

        return await repository.AddAsync(report);
    }

    public async Task CloseReportAsync(int reportId)
    {
        ReportModel? report = await repository.GetByIdAsync(reportId);
        if (report == null)
            throw new NotImplementedException();

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
