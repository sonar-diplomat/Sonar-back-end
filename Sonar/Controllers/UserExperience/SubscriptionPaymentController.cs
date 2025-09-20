using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPaymentController : ControllerBase
    {
        private readonly SonarContext _context;

        public SubscriptionPaymentController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/SubscriptionPayment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionPayment>>> GetSubscriptionPayments()
        {
            return await _context.SubscriptionPayments.ToListAsync();
        }

        // GET: api/SubscriptionPayment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPayment>> GetSubscriptionPayment(int id)
        {
            var subscriptionPayment = await _context.SubscriptionPayments.FindAsync(id);

            if (subscriptionPayment == null)
            {
                return NotFound();
            }

            return subscriptionPayment;
        }

        // PUT: api/SubscriptionPayment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscriptionPayment(int id, SubscriptionPayment subscriptionPayment)
        {
            if (id != subscriptionPayment.Id)
            {
                return BadRequest();
            }

            _context.Entry(subscriptionPayment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionPaymentExists(id))
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

        // POST: api/SubscriptionPayment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SubscriptionPayment>> PostSubscriptionPayment(SubscriptionPayment subscriptionPayment)
        {
            _context.SubscriptionPayments.Add(subscriptionPayment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubscriptionPayment", new { id = subscriptionPayment.Id }, subscriptionPayment);
        }

        // DELETE: api/SubscriptionPayment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscriptionPayment(int id)
        {
            var subscriptionPayment = await _context.SubscriptionPayments.FindAsync(id);
            if (subscriptionPayment == null)
            {
                return NotFound();
            }

            _context.SubscriptionPayments.Remove(subscriptionPayment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionPaymentExists(int id)
        {
            return _context.SubscriptionPayments.Any(e => e.Id == id);
        }
    }
}
