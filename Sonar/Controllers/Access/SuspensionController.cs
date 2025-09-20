using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuspensionController : ControllerBase
    {
        private readonly SonarContext _context;

        public SuspensionController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/Suspension
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suspension>>> GetSuspension()
        {
            return await _context.Suspensions.ToListAsync();
        }

        // GET: api/Suspension/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Suspension>> GetSuspension(int id)
        {
            var suspension = await _context.Suspensions.FindAsync(id);

            if (suspension == null)
            {
                return NotFound();
            }

            return suspension;
        }

        // PUT: api/Suspension/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuspension(int id, Suspension suspension)
        {
            if (id != suspension.Id)
            {
                return BadRequest();
            }

            _context.Entry(suspension).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuspensionExists(id))
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

        // POST: api/Suspension
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Suspension>> PostSuspension(Suspension suspension)
        {
            _context.Suspensions.Add(suspension);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuspension", new { id = suspension.Id }, suspension);
        }

        // DELETE: api/Suspension/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuspension(int id)
        {
            var suspension = await _context.Suspensions.FindAsync(id);
            if (suspension == null)
            {
                return NotFound();
            }

            _context.Suspensions.Remove(suspension);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SuspensionExists(int id)
        {
            return _context.Suspensions.Any(e => e.Id == id);
        }
    }
}
