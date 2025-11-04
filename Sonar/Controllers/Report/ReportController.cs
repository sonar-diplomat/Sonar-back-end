using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Report;
using Application.DTOs.User;
using Application.Response;
using Entities.Enums;
using Entities.Models.Report;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// Retrieves all reports in the system.
    /// </summary>
    /// <returns>List of report DTOs.</returns>
    /// <response code="200">Reports retrieved successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ReportDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetReports()
    {
        IEnumerable<ReportModel> reports = await reportService.GetAllAsync();
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves a specific report by its ID.
    /// </summary>
    /// <param name="id">The ID of the report to retrieve.</param>
    /// <returns>Report DTO with full details.</returns>
    /// <response code="200">Report retrieved successfully.</response>
    /// <response code="404">Report not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OkResponse<ReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportDTO>> GetReport(int id)
    {
        ReportModel report = await reportService.GetByIdValidatedAsync(id);
        ReportDTO dto = MapToDTO(report);
        throw ResponseFactory.Create<OkResponse<ReportDTO>>(dto, ["Report retrieved successfully"]);
    }

    /// <summary>
    /// Deletes a report from the system.
    /// </summary>
    /// <param name="id">The ID of the report to delete.</param>
    /// <returns>Success response upon deletion.</returns>
    /// <response code="200">Report deleted successfully.</response>
    /// <response code="401">User not authorized (requires 'ManageReports' access feature).</response>
    /// <response code="404">Report not found.</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReport(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        await reportService.DeleteAsync(id);
        throw ResponseFactory.Create<OkResponse>(["User deleted successfully"]);
    }

    #endregion

    #region Business-Specific Report Endpoints

    /// <summary>
    /// Creates a new report for content violation or abuse.
    /// </summary>
    /// <param name="dto">Report creation DTO containing entity ID, type, and reason types.</param>
    /// <returns>Created report DTO.</returns>
    /// <response code="200">Report created successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ReportContent' access feature.</response>
    /// <response code="400">Invalid report data.</response>
    /// <remarks>
    /// Requires 'ReportContent' access feature.
    /// </remarks>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<ReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReportDTO>> CreateReport([FromBody] CreateReportDTO dto)
    {
        User user = await CheckAccessFeatures([AccessFeatureStruct.ReportContent]);
        ReportModel report = await reportService.CreateReportAsync(user.Id, dto);
        ReportDTO responseDto = MapToDTO(report);
        throw ResponseFactory.Create<OkResponse<ReportDTO>>(responseDto, ["Report created successfully"]);
    }

    /// <summary>
    /// Marks a report as closed after review.
    /// </summary>
    /// <param name="id">The ID of the report to close.</param>
    /// <returns>Success response upon closing the report.</returns>
    /// <response code="200">Report closed successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ManageReports' access feature.</response>
    /// <response code="404">Report not found.</response>
    /// <remarks>
    /// Requires 'ManageReports' access feature.
    /// </remarks>
    [HttpPut("{id}/close")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseReport(int id)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        await reportService.CloseReportAsync(id);
        throw ResponseFactory.Create<OkResponse>(["Report closed successfully"]);
    }

    /// <summary>
    /// Retrieves reports filtered by various criteria (status, type, date range, etc.).
    /// </summary>
    /// <param name="dto">Report filter DTO with filter criteria.</param>
    /// <returns>Filtered list of report DTOs.</returns>
    /// <response code="200">Reports retrieved successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ManageReports' access feature.</response>
    /// <remarks>
    /// Requires 'ManageReports' access feature.
    /// </remarks>
    [HttpGet("filter")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ReportDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetFiltered([FromBody] ReportFilterDTO dto)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports = await reportService.GetReportsFilteredAsync(dto);
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves all reports submitted by a specific user.
    /// </summary>
    /// <param name="reporterId">The ID of the reporter user.</param>
    /// <returns>List of report DTOs submitted by the specified user.</returns>
    /// <response code="200">Reports retrieved successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ManageReports' access feature.</response>
    /// <remarks>
    /// Requires 'ManageReports' access feature.
    /// </remarks>
    [HttpGet("reporter/{reporterId}")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ReportDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetReportsByReporter(int reporterId)
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports = await reportService.GetReportsByReporterAsync(reporterId);
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    /// <summary>
    /// Retrieves all open (unresolved) reports in the system.
    /// </summary>
    /// <returns>List of open report DTOs.</returns>
    /// <response code="200">Open reports retrieved successfully.</response>
    /// <response code="401">User not authenticated or lacks 'ManageReports' access feature.</response>
    /// <remarks>
    /// Requires 'ManageReports' access feature.
    /// </remarks>
    [HttpGet("open")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ReportDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetOpenReports()
    {
        await CheckAccessFeatures([AccessFeatureStruct.ManageReports]);
        IEnumerable<ReportModel> reports = await reportService.GetOpenReportsAsync();
        IEnumerable<ReportDTO> dtos = reports.Select(MapToDTO);
        throw ResponseFactory.Create<OkResponse<IEnumerable<ReportDTO>>>(dtos, ["Reports retrieved successfully"]);
    }

    #endregion

    #region Report Reason Type Endpoints

    /// <summary>
    /// Retrieves all available report reason types (e.g., spam, harassment, inappropriate content).
    /// </summary>
    /// <returns>List of report reason type DTOs with recommended suspension durations.</returns>
    /// <response code="200">Report reason types retrieved successfully.</response>
    [HttpGet("reason-types")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ReportReasonTypeDTO>>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Retrieves a specific report reason type by its ID.
    /// </summary>
    /// <param name="id">The ID of the report reason type.</param>
    /// <returns>Report reason type DTO with details and recommended suspension duration.</returns>
    /// <response code="200">Report reason type retrieved successfully.</response>
    /// <response code="404">Report reason type not found.</response>
    [HttpGet("reason-types/{id}")]
    [ProducesResponseType(typeof(OkResponse<ReportReasonTypeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Retrieves all reportable entity types (e.g., user, track, album, playlist).
    /// </summary>
    /// <returns>List of reportable entity type DTOs.</returns>
    /// <response code="200">Reportable entity types retrieved successfully.</response>
    [HttpGet("entity-types")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<ReportableEntityTypeDTO>>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Retrieves a specific reportable entity type by its ID.
    /// </summary>
    /// <param name="id">The ID of the reportable entity type.</param>
    /// <returns>Reportable entity type DTO.</returns>
    /// <response code="200">Reportable entity type retrieved successfully.</response>
    /// <response code="404">Reportable entity type not found.</response>
    [HttpGet("entity-types/{id}")]
    [ProducesResponseType(typeof(OkResponse<ReportableEntityTypeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
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