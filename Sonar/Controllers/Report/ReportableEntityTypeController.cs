using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportableEntityTypeController : ControllerBase
    {
        private readonly SonarContext _context;

        public ReportableEntityTypeController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/ReportableEntityType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportableEntityType>>> GetReportableEntityTypes()
        {
            return await _context.ReportableEntityTypes.ToListAsync();
        }

        // GET: api/ReportableEntityType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportableEntityType>> GetReportableEntityType(int id)
        {
            var reportableEntityType = await _context.ReportableEntityTypes.FindAsync(id);

            if (reportableEntityType == null)
            {
                return NotFound();
            }

            return reportableEntityType;
        }

        // PUT: api/ReportableEntityType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportableEntityType(int id, ReportableEntityType reportableEntityType)
        {
            if (id != reportableEntityType.Id)
            {
                return BadRequest();
            }

            _context.Entry(reportableEntityType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportableEntityTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ReportableEntityType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ReportableEntityType>> PostReportableEntityType(ReportableEntityType reportableEntityType)
        {
            _context.ReportableEntityTypes.Add(reportableEntityType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportableEntityType", new { id = reportableEntityType.Id }, reportableEntityType);
        }

        // DELETE: api/ReportableEntityType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportableEntityType(int id)
        {
            var reportableEntityType = await _context.ReportableEntityTypes.FindAsync(id);
            if (reportableEntityType == null)
            {
                return NotFound();
            }

            _context.ReportableEntityTypes.Remove(reportableEntityType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReportableEntityTypeExists(int id)
        {
            return _context.ReportableEntityTypes.Any(e => e.Id == id);
        }
    }
}
