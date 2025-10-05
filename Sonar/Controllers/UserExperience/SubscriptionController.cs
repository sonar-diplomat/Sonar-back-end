using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly SonarContext _context;

        public SubscriptionController(SonarContext context)
        {
            _context = context;
        }

        #region SubscriptionFeature CRUD

        // GET: api/Subscription/Feature
        [HttpGet("Feature")]
        public async Task<ActionResult<IEnumerable<SubscriptionFeature>>> GetSubscriptionFeatures()
        {
            return await _context.SubscriptionFeatures
                .Include(sf => sf.SubscriptionPacks)
                .ToListAsync();
        }

        // GET: api/Subscription/Feature/5
        [HttpGet("Feature/{id}")]
        public async Task<ActionResult<SubscriptionFeature>> GetSubscriptionFeature(int id)
        {
            var subscriptionFeature = await _context.SubscriptionFeatures
                .Include(sf => sf.SubscriptionPacks)
                .FirstOrDefaultAsync(sf => sf.Id == id);

            if (subscriptionFeature == null)
            {
                return NotFound();
            }

            return subscriptionFeature;
        }

        // PUT: api/Subscription/Feature/5
        [HttpPut("Feature/{id}")]
        public async Task<IActionResult> PutSubscriptionFeature(int id, SubscriptionFeature subscriptionFeature)
        {
            if (id != subscriptionFeature.Id)
            {
                return BadRequest();
            }

            _context.Entry(subscriptionFeature).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionFeatureExists(id))
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

        // POST: api/Subscription/Feature
        [HttpPost("Feature")]
        public async Task<ActionResult<SubscriptionFeature>> PostSubscriptionFeature(SubscriptionFeature subscriptionFeature)
        {
            _context.SubscriptionFeatures.Add(subscriptionFeature);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubscriptionFeature", new { id = subscriptionFeature.Id }, subscriptionFeature);
        }

        // DELETE: api/Subscription/Feature/5
        [HttpDelete("Feature/{id}")]
        public async Task<IActionResult> DeleteSubscriptionFeature(int id)
        {
            var subscriptionFeature = await _context.SubscriptionFeatures.FindAsync(id);
            if (subscriptionFeature == null)
            {
                return NotFound();
            }

            _context.SubscriptionFeatures.Remove(subscriptionFeature);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionFeatureExists(int id)
        {
            return _context.SubscriptionFeatures.Any(e => e.Id == id);
        }

        #endregion

        #region SubscriptionPack CRUD

        // GET: api/Subscription/Pack
        [HttpGet("Pack")]
        public async Task<ActionResult<IEnumerable<SubscriptionPack>>> GetSubscriptionPacks()
        {
            return await _context.SubscriptionPacks
                .Include(sp => sp.SubscriptionFeatures)
                .ToListAsync();
        }

        // GET: api/Subscription/Pack/5
        [HttpGet("Pack/{id}")]
        public async Task<ActionResult<SubscriptionPack>> GetSubscriptionPack(int id)
        {
            var subscriptionPack = await _context.SubscriptionPacks
                .Include(sp => sp.SubscriptionFeatures)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (subscriptionPack == null)
            {
                return NotFound();
            }

            return subscriptionPack;
        }

        // PUT: api/Subscription/Pack/5
        [HttpPut("Pack/{id}")]
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

        // POST: api/Subscription/Pack
        [HttpPost("Pack")]
        public async Task<ActionResult<SubscriptionPack>> PostSubscriptionPack(SubscriptionPack subscriptionPack)
        {
            _context.SubscriptionPacks.Add(subscriptionPack);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubscriptionPack", new { id = subscriptionPack.Id }, subscriptionPack);
        }

        // DELETE: api/Subscription/Pack/5
        [HttpDelete("Pack/{id}")]
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

        #endregion

        #region SubscriptionPayment CRUD

        // GET: api/Subscription/Payment
        [HttpGet("Payment")]
        public async Task<ActionResult<IEnumerable<SubscriptionPayment>>> GetSubscriptionPayments()
        {
            return await _context.SubscriptionPayments
                .Include(sp => sp.Buyer)
                .Include(sp => sp.SubscriptionPack)
                    .ThenInclude(pack => pack.SubscriptionFeatures)
                .ToListAsync();
        }

        // GET: api/Subscription/Payment/5
        [HttpGet("Payment/{id}")]
        public async Task<ActionResult<SubscriptionPayment>> GetSubscriptionPayment(int id)
        {
            var subscriptionPayment = await _context.SubscriptionPayments
                .Include(sp => sp.Buyer)
                .Include(sp => sp.SubscriptionPack)
                    .ThenInclude(pack => pack.SubscriptionFeatures)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (subscriptionPayment == null)
            {
                return NotFound();
            }

            return subscriptionPayment;
        }

        // GET: api/Subscription/Payment/user/5
        [HttpGet("Payment/user/{userId}")]
        public async Task<ActionResult<IEnumerable<SubscriptionPayment>>> GetSubscriptionPaymentsByUser(int userId)
        {
            return await _context.SubscriptionPayments
                .Include(sp => sp.Buyer)
                .Include(sp => sp.SubscriptionPack)
                    .ThenInclude(pack => pack.SubscriptionFeatures)
                .Where(sp => sp.BuyerId == userId)
                .ToListAsync();
        }

        // PUT: api/Subscription/Payment/5
        [HttpPut("Payment/{id}")]
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

        // POST: api/Subscription/Payment
        [HttpPost("Payment")]
        public async Task<ActionResult<SubscriptionPayment>> PostSubscriptionPayment(SubscriptionPayment subscriptionPayment)
        {
            _context.SubscriptionPayments.Add(subscriptionPayment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubscriptionPayment", new { id = subscriptionPayment.Id }, subscriptionPayment);
        }

        // DELETE: api/Subscription/Payment/5
        [HttpDelete("Payment/{id}")]
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

        #endregion
    }
}
