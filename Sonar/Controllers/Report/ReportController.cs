using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Report;
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
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetReports()
    {
        IEnumerable<ReportModel> reports = await reportService.GetAllAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportModel>>>(reports, ["Reports retrieved successfully"]);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReportModel>> GetReport(int id)
    {
        ReportModel report = await reportService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<ReportModel>>(report, ["Report retrieved successfully"]);
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
    public async Task<ActionResult<ReportModel>> CreateReport([FromBody] CreateReportDTO dto)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ReportContent]);
        ReportModel report = await reportService.CreateReportAsync(user.Id, dto);
        throw ResponseFactory.Create<OkResponse<ReportModel>>(report, ["Report created successfully"]);
    }

    [HttpPut("{id}/close")]
    public async Task<IActionResult> CloseReport(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        await reportService.CloseReportAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Report closed successfully"]);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetFiltered([FromBody] ReportFilterDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports =
            await reportService.GetReportsFilteredAsync(dto);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportModel>>>(reports, ["Reports retrieved successfully"]);
    }

    [HttpGet("reporter/{reporterId}")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetReportsByReporter(int reporterId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports =
            await reportService.GetReportsByReporterAsync(reporterId);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportModel>>>(reports, ["Reports retrieved successfully"]);
    }

    [HttpGet("open")]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetOpenReports()
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports = await reportService.GetOpenReportsAsync();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportModel>>>(reports, ["Reports retrieved successfully"]);
    }

    #endregion

    #region Report Reason Type Endpoints

    [HttpGet("reason-types")]
    public async Task<ActionResult<IEnumerable<ReportReasonType>>> GetReasonTypes()
    {
        IEnumerable<ReportReasonType> reasonTypes =
            (await reportReasonTypeService.GetAllAsync()).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportReasonType>>>(reasonTypes,
            ["Reason types retrieved successfully"]);
    }

    [HttpGet("reason-types/{id}")]
    public async Task<ActionResult<ReportReasonType>> GetReasonType(int id)
    {
        ReportReasonType reasonType = await reportReasonTypeService.GetByIdValidatedAsync(id);
        throw ResponseFactory.Create<OkResponse<ReportReasonType>>(reasonType, ["Reason type retrieved successfully"]);
    }

    #endregion

    #region Reportable Entity Type Endpoints

    [HttpGet("entity-types")]
    public async Task<ActionResult<IEnumerable<ReportableEntityType>>> GetEntityTypes()
    {
        IEnumerable<ReportableEntityType> entityTypes =
            (await reportableEntityTypeService.GetAllAsync()).ToList();
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportableEntityType>>>(entityTypes,
            ["Entity types retrieved successfully"]);
    }

    [HttpGet("entity-types/{id}")]
    public async Task<ActionResult<ReportableEntityType>> GetEntityType(int id)
    {
        ReportableEntityType entityType =
            await reportableEntityTypeService.GetByIdValidatedAsync(id);

        throw ResponseFactory.Create<OkResponse<ReportableEntityType>>(entityType,
            ["Entity type retrieved successfully"]);
    }

    #endregion
}