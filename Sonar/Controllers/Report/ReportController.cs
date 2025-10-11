using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportService reportService;
    private readonly IReportReasonTypeService reportReasonTypeService;
    private readonly IReportableEntityTypeService reportableEntityTypeService;
    private readonly AppExceptionFactory appExceptionFactory;

    public ReportController(
        IReportService reportService,
        IReportReasonTypeService reportReasonTypeService,
        IReportableEntityTypeService reportableEntityTypeService,
        AppExceptionFactory appExceptionFactory)
    {
        this.reportService = reportService;
        this.reportReasonTypeService = reportReasonTypeService;
        this.reportableEntityTypeService = reportableEntityTypeService;
        this.appExceptionFactory = appExceptionFactory;
    }

    #region Report CRUD Endpoints

    /// <summary>
    ///     Get all reports
    /// </summary>
    /// <returns>List of all reports</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entities.Models.Report.Report>>> GetReports()
    {
        try
        {
            IEnumerable<Entities.Models.Report.Report> reports = await reportService.GetAllAsync();
            return Ok(reports);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get a specific report by ID
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <returns>Report details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Entities.Models.Report.Report>> GetReport(int id)
    {
        try
        {
            Entities.Models.Report.Report report = await reportService.GetByIdAsync(id);

            if (report == null)
                throw new NotImplementedException();

            return Ok(report);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Update a report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="report">Updated report details</param>
    /// <returns>Updated report</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<Entities.Models.Report.Report>> PutReport(int id,
        [FromBody] Entities.Models.Report.Report report)
    {
        try
        {
            if (id != report.Id)
                throw new NotImplementedException();

            Entities.Models.Report.Report updatedReport = await reportService.UpdateAsync(report);
            return Ok(updatedReport);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Create a new report (direct entity creation - prefer using /create endpoint)
    /// </summary>
    /// <param name="report">Report details</param>
    /// <returns>Created a report</returns>
    [HttpPost]
    public async Task<ActionResult<Entities.Models.Report.Report>> PostReport(
        [FromBody] Entities.Models.Report.Report report)
    {
        try
        {
            Entities.Models.Report.Report createdReport = await reportService.CreateAsync(report);
            return CreatedAtAction(nameof(GetReport), new { id = createdReport.Id }, createdReport);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Delete a report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReport(int id)
    {
        try
        {
            await reportService.DeleteAsync(id);
            return NoContent();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Business-Specific Report Endpoints

    /// <summary>
    ///     Create a report for an entity (user, post, song, etc.)
    /// </summary>
    /// <param name="dto">Report creation details</param>
    /// <returns>Created report</returns>
    [HttpPost("create")]
    public async Task<ActionResult<Entities.Models.Report.Report>> CreateReport([FromBody] CreateReportDTO dto)
    {
        try
        {
            Entities.Models.Report.Report report = await reportService.CreateReportAsync(dto);
            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Close/resolve a report (moderator action)
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/close")]
    public async Task<IActionResult> CloseReport(int id)
    {
        try
        {
            await reportService.CloseReportAsync(id);
            return NoContent();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get all reports for a specific entity
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="typeId">Reportable entity type ID</param>
    /// <returns>List of reports for the entity</returns>
    [HttpGet("entity/{entityId}/type/{typeId}")]
    public async Task<ActionResult<IEnumerable<Entities.Models.Report.Report>>> GetReportsByEntity(int entityId,
        int typeId)
    {
        try
        {
            IEnumerable<Entities.Models.Report.Report> reports =
                await reportService.GetReportsByEntityAsync(entityId, typeId);
            return Ok(reports);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get all reports submitted by a specific user
    /// </summary>
    /// <param name="reporterId">Reporter user ID</param>
    /// <returns>List of reports by the user</returns>
    [HttpGet("reporter/{reporterId}")]
    public async Task<ActionResult<IEnumerable<Entities.Models.Report.Report>>> GetReportsByReporter(int reporterId)
    {
        try
        {
            IEnumerable<Entities.Models.Report.Report> reports =
                await reportService.GetReportsByReporterAsync(reporterId);
            return Ok(reports);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get all open/unresolved reports (moderation queue)
    /// </summary>
    /// <returns>List of open reports</returns>
    [HttpGet("open")]
    public async Task<ActionResult<IEnumerable<Entities.Models.Report.Report>>> GetOpenReports()
    {
        try
        {
            IEnumerable<Entities.Models.Report.Report> reports = await reportService.GetOpenReportsAsync();
            return Ok(reports);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Report Reason Type Endpoints

    /// <summary>
    ///     Get all report reason types
    /// </summary>
    /// <returns>List of all reason types</returns>
    [HttpGet("reason-types")]
    public async Task<ActionResult<IEnumerable<Entities.Models.Report.ReportReasonType>>> GetReasonTypes()
    {
        try
        {
            IEnumerable<Entities.Models.Report.ReportReasonType> reasonTypes =
                await reportReasonTypeService.GetAllAsync();
            return Ok(reasonTypes);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get a specific report reason type by ID
    /// </summary>
    /// <param name="id">Reason type ID</param>
    /// <returns>Reason type details</returns>
    [HttpGet("reason-types/{id}")]
    public async Task<ActionResult<Entities.Models.Report.ReportReasonType>> GetReasonType(int id)
    {
        try
        {
            Entities.Models.Report.ReportReasonType reasonType =
                await reportReasonTypeService.GetByIdAsync(id);

            if (reasonType == null)
                throw new NotImplementedException();

            return Ok(reasonType);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Create a new report reason type (admin only)
    /// </summary>
    /// <param name="reasonType">Reason type details</param>
    /// <returns>Created reason type</returns>
    [HttpPost("reason-types")]
    public async Task<ActionResult<Entities.Models.Report.ReportReasonType>> CreateReasonType(
        [FromBody] Entities.Models.Report.ReportReasonType reasonType)
    {
        try
        {
            Entities.Models.Report.ReportReasonType createdReasonType =
                await reportReasonTypeService.CreateAsync(reasonType);
            return CreatedAtAction(nameof(GetReasonType), new { id = createdReasonType.Id }, createdReasonType);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Update a report reason type (admin only)
    /// </summary>
    /// <param name="id">Reason type ID</param>
    /// <param name="reasonType">Updated reason type details</param>
    /// <returns>Updated reason type</returns>
    [HttpPut("reason-types/{id}")]
    public async Task<ActionResult<Entities.Models.Report.ReportReasonType>> UpdateReasonType(int id,
        [FromBody] Entities.Models.Report.ReportReasonType reasonType)
    {
        try
        {
            if (id != reasonType.Id)
                throw new NotImplementedException();

            Entities.Models.Report.ReportReasonType updatedReasonType =
                await reportReasonTypeService.UpdateAsync(reasonType);
            return Ok(updatedReasonType);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Delete a report reason type (admin only)
    /// </summary>
    /// <param name="id">Reason type ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("reason-types/{id}")]
    public async Task<IActionResult> DeleteReasonType(int id)
    {
        try
        {
            await reportReasonTypeService.DeleteAsync(id);
            return NoContent();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Reportable Entity Type Endpoints

    /// <summary>
    ///     Get all reportable entity types
    /// </summary>
    /// <returns>List of all entity types</returns>
    [HttpGet("entity-types")]
    public async Task<ActionResult<IEnumerable<Entities.Models.Report.ReportableEntityType>>> GetEntityTypes()
    {
        try
        {
            IEnumerable<Entities.Models.Report.ReportableEntityType> entityTypes =
                await reportableEntityTypeService.GetAllAsync();
            return Ok(entityTypes);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Get a specific reportable entity type by ID
    /// </summary>
    /// <param name="id">Entity type ID</param>
    /// <returns>Entity type details</returns>
    [HttpGet("entity-types/{id}")]
    public async Task<ActionResult<Entities.Models.Report.ReportableEntityType>> GetEntityType(int id)
    {
        try
        {
            Entities.Models.Report.ReportableEntityType entityType =
                await reportableEntityTypeService.GetByIdAsync(id);

            if (entityType == null)
                throw new NotImplementedException();

            return Ok(entityType);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Create a new reportable entity type (admin only)
    /// </summary>
    /// <param name="entityType">Entity type details</param>
    /// <returns>Created entity type</returns>
    [HttpPost("entity-types")]
    public async Task<ActionResult<Entities.Models.Report.ReportableEntityType>> CreateEntityType(
        [FromBody] Entities.Models.Report.ReportableEntityType entityType)
    {
        try
        {
            Entities.Models.Report.ReportableEntityType createdEntityType =
                await reportableEntityTypeService.CreateAsync(entityType);
            return CreatedAtAction(nameof(GetEntityType), new { id = createdEntityType.Id }, createdEntityType);
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Update a reportable entity type (admin only)
    /// </summary>
    /// <param name="id">Entity type ID</param>
    /// <param name="entityType">Updated entity type details</param>
    /// <returns>Updated entity type</returns>
    [HttpPut("entity-types/{id}")]
    public async Task<ActionResult<Entities.Models.Report.ReportableEntityType>> UpdateEntityType(int id,
        [FromBody] Entities.Models.Report.ReportableEntityType entityType)
    {
        try
        {
            if (id != entityType.Id)
                throw new NotImplementedException();

            Entities.Models.Report.ReportableEntityType updatedEntityType =
                await reportableEntityTypeService.UpdateAsync(entityType);
            return Ok(updatedEntityType);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Delete a reportable entity type (admin only)
    /// </summary>
    /// <param name="id">Entity type ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("entity-types/{id}")]
    public async Task<IActionResult> DeleteEntityType(int id)
    {
        try
        {
            await reportableEntityTypeService.DeleteAsync(id);
            return NoContent();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
