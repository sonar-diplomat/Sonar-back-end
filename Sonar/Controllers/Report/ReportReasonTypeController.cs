using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportReasonTypeController : ControllerBase
    {
        private readonly SonarContext _context;

        public ReportReasonTypeController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/ReportReasonType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportReasonType>>> GetReportReasonTypes()
        {
            return await _context.ReportReasonTypes.ToListAsync();
        }

        // GET: api/ReportReasonType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportReasonType>> GetReportReasonType(int id)
        {
            var reportReasonType = await _context.ReportReasonTypes.FindAsync(id);

            if (reportReasonType == null)
            {
                return NotFound();
            }

            return reportReasonType;
        }

        // PUT: api/ReportReasonType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportReasonType(int id, ReportReasonType reportReasonType)
        {
            if (id != reportReasonType.Id)
            {
                return BadRequest();
            }

            _context.Entry(reportReasonType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportReasonTypeExists(id))
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

        // POST: api/ReportReasonType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ReportReasonType>> PostReportReasonType(ReportReasonType reportReasonType)
        {
            _context.ReportReasonTypes.Add(reportReasonType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportReasonType", new { id = reportReasonType.Id }, reportReasonType);
        }

        // DELETE: api/ReportReasonType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportReasonType(int id)
        {
            var reportReasonType = await _context.ReportReasonTypes.FindAsync(id);
            if (reportReasonType == null)
            {
                return NotFound();
            }

            _context.ReportReasonTypes.Remove(reportReasonType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReportReasonTypeExists(int id)
        {
            return _context.ReportReasonTypes.Any(e => e.Id == id);
        }
    }
}
