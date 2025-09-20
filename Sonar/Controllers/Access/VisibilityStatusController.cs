using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.Access
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisibilityStatusController : ControllerBase
    {
        private readonly SonarContext _context;

        public VisibilityStatusController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/VisibilityStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisibilityStatus>>> GetVisibilityStatusses()
        {
            return await _context.VisibilityStatuses.ToListAsync();
        }

        // GET: api/VisibilityStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VisibilityStatus>> GetVisibilityStatus(int id)
        {
            var visibilityStatus = await _context.VisibilityStatuses.FindAsync(id);

            if (visibilityStatus == null)
            {
                return NotFound();
            }

            return visibilityStatus;
        }

        // PUT: api/VisibilityStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVisibilityStatus(int id, VisibilityStatus visibilityStatus)
        {
            if (id != visibilityStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(visibilityStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VisibilityStatusExists(id))
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

        // POST: api/VisibilityStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VisibilityStatus>> PostVisibilityStatus(VisibilityStatus visibilityStatus)
        {
            _context.VisibilityStatuses.Add(visibilityStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVisibilityStatus", new { id = visibilityStatus.Id }, visibilityStatus);
        }

        // DELETE: api/VisibilityStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisibilityStatus(int id)
        {
            var visibilityStatus = await _context.VisibilityStatuses.FindAsync(id);
            if (visibilityStatus == null)
            {
                return NotFound();
            }

            _context.VisibilityStatuses.Remove(visibilityStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VisibilityStatusExists(int id)
        {
            return _context.VisibilityStatuses.Any(e => e.Id == id);
        }
    }
}
