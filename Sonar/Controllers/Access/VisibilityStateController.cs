using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisibilityStateController : ControllerBase
    {
        private readonly SonarContext _context;

        public VisibilityStateController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/VisibilityState
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisibilityState>>> GetVisibilityState()
        {
            return await _context.VisibilityStates.ToListAsync();
        }

        // GET: api/VisibilityState/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VisibilityState>> GetVisibilityState(int id)
        {
            var visibilityState = await _context.VisibilityStates.FindAsync(id);

            if (visibilityState == null)
            {
                return NotFound();
            }

            return visibilityState;
        }

        // PUT: api/VisibilityState/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVisibilityState(int id, VisibilityState visibilityState)
        {
            if (id != visibilityState.Id)
            {
                return BadRequest();
            }

            _context.Entry(visibilityState).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VisibilityStateExists(id))
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

        // POST: api/VisibilityState
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VisibilityState>> PostVisibilityState(VisibilityState visibilityState)
        {
            _context.VisibilityStates.Add(visibilityState);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVisibilityState", new { id = visibilityState.Id }, visibilityState);
        }

        // DELETE: api/VisibilityState/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisibilityState(int id)
        {
            var visibilityState = await _context.VisibilityStates.FindAsync(id);
            if (visibilityState == null)
            {
                return NotFound();
            }

            _context.VisibilityStates.Remove(visibilityState);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VisibilityStateExists(int id)
        {
            return _context.VisibilityStates.Any(e => e.Id == id);
        }
    }
}
