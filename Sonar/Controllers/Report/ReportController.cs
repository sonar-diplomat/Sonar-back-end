using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.Report;
using Entities.Models.UserCore;
using Entities.TemplateResponses;
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
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetReports()
    {
        IEnumerable<ReportModel> reports = await reportService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<ReportModel>>(reports));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReportModel>> GetReport(int id)
    {
        ReportModel report = await reportService.GetByIdValidatedAsync(id);
        return Ok(report);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReport(int id)
    {
        await reportService.DeleteAsync(id);
        return Ok(new BaseResponse<bool>(true, "User deleted successfully"));
    }

    #endregion

    #region Business-Specific Report Endpoints

    [HttpPost]
    public async Task<ActionResult<ReportModel>> CreateReport([FromBody] CreateReportDTO dto)
    {
        ReportModel report = await reportService.CreateReportAsync(dto);
        return CreatedAtAction(nameof(GetReport), new { id = report.Id }, new BaseResponse<ReportModel>(report, "Report created successfully"));
    }

    [HttpPut("{id}/close")]
    public async Task<IActionResult> CloseReport(int id)
    {
        await reportService.CloseReportAsync(id);
        return Ok(new BaseResponse<bool>("Report closed successfully"));
    }

    [HttpGet("entity/{entityId}/type/{typeId}")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetReportsByEntity(int entityId,
        int typeId)
    {
        IEnumerable<ReportModel> reports =
            await reportService.GetReportsByEntityAsync(entityId, typeId);
        return Ok(new BaseResponse<IEnumerable<ReportModel>>(reports, "Reports retrieved successfully"));
    }

    [HttpGet("reporter/{reporterId}")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetReportsByReporter(int reporterId)
    {
        IEnumerable<ReportModel> reports =
            await reportService.GetReportsByReporterAsync(reporterId);
        return Ok(new BaseResponse<IEnumerable<ReportModel>>(reports, "Reports retrieved successfully"));
    }

    [HttpGet("open")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetOpenReports()
    {
        IEnumerable<ReportModel> reports = await reportService.GetOpenReportsAsync();
        return Ok(new BaseResponse<IEnumerable<ReportModel>>(reports, "Reports retrieved successfully"));
    }

    #endregion

    #region Report Reason Type Endpoints

    [HttpGet("reason-types")]
    public async Task<ActionResult<IEnumerable<ReportReasonType>>> GetReasonTypes()
    {
        IEnumerable<ReportReasonType> reasonTypes =
            await reportReasonTypeService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<ReportReasonType>>(reasonTypes, "Reason types retrieved successfully"));
    }

    [HttpGet("reason-types/{id}")]
    public async Task<ActionResult<ReportReasonType>> GetReasonType(int id)
    {
        ReportReasonType reasonType =
            await reportReasonTypeService.GetByIdValidatedAsync(id);

        return Ok(new BaseResponse<ReportReasonType>(reasonType, "Reason type retrieved successfully"));
    }

    #endregion

    #region Reportable Entity Type Endpoints

    [HttpGet("entity-types")]
    public async Task<ActionResult<IEnumerable<ReportableEntityType>>> GetEntityTypes()
    {
        IEnumerable<ReportableEntityType> entityTypes =
            await reportableEntityTypeService.GetAllAsync();
        return Ok(new BaseResponse<IEnumerable<ReportableEntityType>>(entityTypes));
    }

    [HttpGet("entity-types/{id}")]
    public async Task<ActionResult<ReportableEntityType>> GetEntityType(int id)
    {
        ReportableEntityType entityType =
            await reportableEntityTypeService.GetByIdValidatedAsync(id);

        return Ok(new BaseResponse<ReportableEntityType>(entityType, "Entity type retrieved successfully"));
    }

    #endregion
}
