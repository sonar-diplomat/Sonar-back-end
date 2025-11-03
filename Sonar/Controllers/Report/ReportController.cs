using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Report;
using Application.DTOs.User;
using Application.Response;
using Entities.Enums;
using Entities.Models.Report;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReportModel = Entities.Models.Report.Report;

namespace Sonar.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class ReportController(
    IReportService reportService,
    IReportReasonTypeService reportReasonTypeService,
    IReportableEntityTypeService reportableEntityTypeService,
    UserManager<User> userManager)
    : BaseController(userManager)
{
    #region Report CRUD Endpoints

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetReports()
    {
        IEnumerable<ReportModel> reports = await reportService.GetAllAsync();
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReportDTO>> GetReport(int id)
    {
        ReportModel report = await reportService.GetByIdValidatedAsync(id);
        ReportDTO dto = MapToDTO(report);
        throw ResponseFactory.Create<OkResponse<ReportDTO>>(dto, ["Report retrieved successfully"]);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReport(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        await reportService.DeleteAsync(id);
        throw ResponseFactory.Create<OkResponse>(["User deleted successfully"]);
    }

    #endregion

    #region Business-Specific Report Endpoints

    [HttpPost]
    public async Task<ActionResult<ReportDTO>> CreateReport([FromBody] CreateReportDTO dto)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ReportContent]);
        ReportModel report = await reportService.CreateReportAsync(user.Id, dto);
        ReportDTO responseDto = MapToDTO(report);
        throw ResponseFactory.Create<OkResponse<ReportDTO>>(responseDto, ["Report created successfully"]);
    }

    [HttpPut("{id}/close")]
    public async Task<IActionResult> CloseReport(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        await reportService.CloseReportAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Report closed successfully"]);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetFiltered([FromBody] ReportFilterDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports = await reportService.GetReportsFilteredAsync(dto);
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    [HttpGet("reporter/{reporterId}")]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetReportsByReporter(int reporterId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports = await reportService.GetReportsByReporterAsync(reporterId);
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    [HttpGet("open")]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetOpenReports()
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports = await reportService.GetOpenReportsAsync();
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    #endregion

    #region Report Reason Type Endpoints

    [HttpGet("reason-types")]
    public async Task<ActionResult<IEnumerable<ReportReasonTypeDTO>>> GetReasonTypes()
    {
        IEnumerable<ReportReasonType> reasonTypes = (await reportReasonTypeService.GetAllAsync()).ToList();
        IEnumerable<ReportReasonTypeDTO> dtos = reasonTypes.Select(rt => new ReportReasonTypeDTO
        {
            Id = rt.Id,
            Name = rt.Name,
            RecommendedSuspensionDuration = rt.RecommendedSuspensionDuration
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportReasonTypeDTO>>>(dtos,
            ["Reason types retrieved successfully"]);
    }

    [HttpGet("reason-types/{id}")]
    public async Task<ActionResult<ReportReasonTypeDTO>> GetReasonType(int id)
    {
        ReportReasonType reasonType = await reportReasonTypeService.GetByIdValidatedAsync(id);
        ReportReasonTypeDTO dto = new()
        {
            Id = reasonType.Id,
            Name = reasonType.Name,
            RecommendedSuspensionDuration = reasonType.RecommendedSuspensionDuration
        };
        throw ResponseFactory.Create<OkResponse<ReportReasonTypeDTO>>(dto, ["Reason type retrieved successfully"]);
    }

    #endregion

    #region Reportable Entity Type Endpoints

    [HttpGet("entity-types")]
    public async Task<ActionResult<IEnumerable<ReportableEntityTypeDTO>>> GetEntityTypes()
    {
        IEnumerable<ReportableEntityType> entityTypes = (await reportableEntityTypeService.GetAllAsync()).ToList();
        IEnumerable<ReportableEntityTypeDTO> dtos = entityTypes.Select(et => new ReportableEntityTypeDTO
        {
            Id = et.Id,
            Name = et.Name
        });
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportableEntityTypeDTO>>>(dtos,
            ["Entity types retrieved successfully"]);
    }

    [HttpGet("entity-types/{id}")]
    public async Task<ActionResult<ReportableEntityTypeDTO>> GetEntityType(int id)
    {
        ReportableEntityType entityType = await reportableEntityTypeService.GetByIdValidatedAsync(id);
        ReportableEntityTypeDTO dto = new()
        {
            Id = entityType.Id,
            Name = entityType.Name
        };
        throw ResponseFactory.Create<OkResponse<ReportableEntityTypeDTO>>(dto,
            ["Entity type retrieved successfully"]);
    }

    #endregion

    private ReportDTO MapToDTO(ReportModel report)
    {
        return new ReportDTO
        {
            Id = report.Id,
            IsClosed = report.IsClosed,
            EntityIdentifier = report.EntityIdentifier,
            CreatedAt = report.CreatedAt,
            Reporter = new UserResponseDTO
            {
                Id = report.Reporter.Id,
                UserName = report.Reporter.UserName ?? string.Empty,
                PublicIdentifier = report.Reporter.PublicIdentifier,
                Biography = report.Reporter.Biography,
                RegistrationDate = report.Reporter.RegistrationDate,
                AvatarUrl = report.Reporter.AvatarImage?.Url ?? string.Empty
            },
            EntityType = new ReportableEntityTypeDTO
            {
                Id = report.ReportableEntityType.Id,
                Name = report.ReportableEntityType.Name
            },
            Reasons = report.ReportReasonType?.Select(r => new ReportReasonTypeDTO
            {
                Id = r.Id,
                Name = r.Name,
                RecommendedSuspensionDuration = r.RecommendedSuspensionDuration
            }).ToList() ?? new List<ReportReasonTypeDTO>()
        };
    }
}