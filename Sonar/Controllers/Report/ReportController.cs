using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly SonarContext _context;

    public ReportController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Report
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entities.Models.Report.Report>>> GetReports()
    {
        return await _context.Reports.ToListAsync();
    }

    // GET: api/Report/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Entities.Models.Report.Report>> GetReport(int id)
    {
        Entities.Models.Report.Report? report = await _context.Reports.FindAsync(id);

        if (report == null) return NotFound();

        return report;
    }

    // PUT: api/Report/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutReport(int id, Entities.Models.Report.Report report)
    {
        if (id != report.Id) return BadRequest();

        _context.Entry(report).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReportExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Report
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Entities.Models.Report.Report>> PostReport(Entities.Models.Report.Report report)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetReport", new { id = report.Id }, report);
    }

    // DELETE: api/Report/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReport(int id)
    {
        Entities.Models.Report.Report? report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ReportExists(int id)
    {
        return _context.Reports.Any(e => e.Id == id);
    }
}