using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftStyleController : ControllerBase
    {
        private readonly SonarContext _context;

        public GiftStyleController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/GiftStyle
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiftStyle>>> GetGiftStyles()
        {
            return await _context.GiftStyles.ToListAsync();
        }

        // GET: api/GiftStyle/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GiftStyle>> GetGiftStyle(int id)
        {
            var giftStyle = await _context.GiftStyles.FindAsync(id);

            if (giftStyle == null)
            {
                return NotFound();
            }

            return giftStyle;
        }

        // PUT: api/GiftStyle/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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

        // POST: api/GiftStyle
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GiftStyle>> PostGiftStyle(GiftStyle giftStyle)
        {
            _context.GiftStyles.Add(giftStyle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGiftStyle", new { id = giftStyle.Id }, giftStyle);
        }

        // DELETE: api/GiftStyle/5
        [HttpDelete("{id}")]
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
    }
}
