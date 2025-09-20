using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.Distribution
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistributorSessionController : ControllerBase
    {
        private readonly SonarContext _context;

        public DistributorSessionController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/DistributorSession
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistributorSession>>> GetDistributorSessions()
        {
            return await _context.DistributorSessions.ToListAsync();
        }

        // GET: api/DistributorSession/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DistributorSession>> GetDistributorSession(int id)
        {
            var distributorSession = await _context.DistributorSessions.FindAsync(id);

            if (distributorSession == null)
            {
                return NotFound();
            }

            return distributorSession;
        }

        // PUT: api/DistributorSession/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDistributorSession(int id, DistributorSession distributorSession)
        {
            if (id != distributorSession.Id)
            {
                return BadRequest();
            }

            _context.Entry(distributorSession).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistributorSessionExists(id))
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

        // POST: api/DistributorSession
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DistributorSession>> PostDistributorSession(DistributorSession distributorSession)
        {
            _context.DistributorSessions.Add(distributorSession);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDistributorSession", new { id = distributorSession.Id }, distributorSession);
        }

        // DELETE: api/DistributorSession/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistributorSession(int id)
        {
            var distributorSession = await _context.DistributorSessions.FindAsync(id);
            if (distributorSession == null)
            {
                return NotFound();
            }

            _context.DistributorSessions.Remove(distributorSession);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DistributorSessionExists(int id)
        {
            return _context.DistributorSessions.Any(e => e.Id == id);
        }
    }
}
