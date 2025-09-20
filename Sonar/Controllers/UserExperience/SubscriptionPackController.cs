using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPackController : ControllerBase
    {
        private readonly SonarContext _context;

        public SubscriptionPackController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/SubscriptionPack
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionPack>>> GetSubscriptionPacks()
        {
            return await _context.SubscriptionPacks.ToListAsync();
        }

        // GET: api/SubscriptionPack/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPack>> GetSubscriptionPack(int id)
        {
            var subscriptionPack = await _context.SubscriptionPacks.FindAsync(id);

            if (subscriptionPack == null)
            {
                return NotFound();
            }

            return subscriptionPack;
        }

        // PUT: api/SubscriptionPack/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscriptionPack(int id, SubscriptionPack subscriptionPack)
        {
            if (id != subscriptionPack.Id)
            {
                return BadRequest();
            }

            _context.Entry(subscriptionPack).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionPackExists(id))
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

        // POST: api/SubscriptionPack
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SubscriptionPack>> PostSubscriptionPack(SubscriptionPack subscriptionPack)
        {
            _context.SubscriptionPacks.Add(subscriptionPack);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubscriptionPack", new { id = subscriptionPack.Id }, subscriptionPack);
        }

        // DELETE: api/SubscriptionPack/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscriptionPack(int id)
        {
            var subscriptionPack = await _context.SubscriptionPacks.FindAsync(id);
            if (subscriptionPack == null)
            {
                return NotFound();
            }

            _context.SubscriptionPacks.Remove(subscriptionPack);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionPackExists(int id)
        {
            return _context.SubscriptionPacks.Any(e => e.Id == id);
        }
    }
}
