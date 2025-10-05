using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models.UserExperience;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly SonarContext _context;

        public GiftController(SonarContext context)
        {
            _context = context;
        }

        #region Gift CRUD

        // GET: api/Gift
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gift>>> GetGifts()
        {
            return await _context.Gifts
                .Include(g => g.Receiver)
                .Include(g => g.GiftStyle)
                .Include(g => g.SubscriptionPayment)
                .ToListAsync();
        }

        // GET: api/Gift/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gift>> GetGift(int id)
        {
            var gift = await _context.Gifts
                .Include(g => g.Receiver)
                .Include(g => g.GiftStyle)
                .Include(g => g.SubscriptionPayment)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gift == null)
            {
                return NotFound();
            }

            return gift;
        }

        // PUT: api/Gift/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGift(int id, Gift gift)
        {
            if (id != gift.Id)
            {
                return BadRequest();
            }

            _context.Entry(gift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GiftExists(id))
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

        // POST: api/Gift
        [HttpPost]
        public async Task<ActionResult<Gift>> PostGift(Gift gift)
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGift", new { id = gift.Id }, gift);
        }

        // DELETE: api/Gift/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null)
            {
                return NotFound();
            }

            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GiftExists(int id)
        {
            return _context.Gifts.Any(e => e.Id == id);
        }

        #endregion

        #region GiftStyle CRUD

        // GET: api/Gift/Style
        [HttpGet("Style")]
        public async Task<ActionResult<IEnumerable<GiftStyle>>> GetGiftStyles()
        {
            return await _context.GiftStyles.ToListAsync();
        }

        // GET: api/Gift/Style/5
        [HttpGet("Style/{id}")]
        public async Task<ActionResult<GiftStyle>> GetGiftStyle(int id)
        {
            var giftStyle = await _context.GiftStyles.FindAsync(id);

            if (giftStyle == null)
            {
                return NotFound();
            }

            return giftStyle;
        }

        // PUT: api/Gift/Style/5
        [HttpPut("Style/{id}")]
        public async Task<IActionResult> PutGiftStyle(int id, GiftStyle giftStyle)
        {
            if (id != giftStyle.Id)
            {
                return BadRequest();
            }

            _context.Entry(giftStyle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GiftStyleExists(id))
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

        // POST: api/Gift/Style
        [HttpPost("Style")]
        public async Task<ActionResult<GiftStyle>> PostGiftStyle(GiftStyle giftStyle)
        {
            _context.GiftStyles.Add(giftStyle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGiftStyle", new { id = giftStyle.Id }, giftStyle);
        }

        // DELETE: api/Gift/Style/5
        [HttpDelete("Style/{id}")]
        public async Task<IActionResult> DeleteGiftStyle(int id)
        {
            var giftStyle = await _context.GiftStyles.FindAsync(id);
            if (giftStyle == null)
            {
                return NotFound();
            }

            _context.GiftStyles.Remove(giftStyle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GiftStyleExists(int id)
        {
            return _context.GiftStyles.Any(e => e.Id == id);
        }

        #endregion
    }
}
