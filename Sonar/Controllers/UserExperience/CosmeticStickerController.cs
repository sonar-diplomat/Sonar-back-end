using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmeticStickerController : ControllerBase
    {
        private readonly SonarContext _context;

        public CosmeticStickerController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/CosmeticSticker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CosmeticSticker>>> GetCosmeticStickers()
        {
            return await _context.CosmeticStickers.ToListAsync();
        }

        // GET: api/CosmeticSticker/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CosmeticSticker>> GetCosmeticSticker(int id)
        {
            var cosmeticSticker = await _context.CosmeticStickers.FindAsync(id);

            if (cosmeticSticker == null)
            {
                return NotFound();
            }

            return cosmeticSticker;
        }

        // PUT: api/CosmeticSticker/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCosmeticSticker(int id, CosmeticSticker cosmeticSticker)
        {
            if (id != cosmeticSticker.Id)
            {
                return BadRequest();
            }

            _context.Entry(cosmeticSticker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CosmeticStickerExists(id))
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

        // POST: api/CosmeticSticker
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CosmeticSticker>> PostCosmeticSticker(CosmeticSticker cosmeticSticker)
        {
            _context.CosmeticStickers.Add(cosmeticSticker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCosmeticSticker", new { id = cosmeticSticker.Id }, cosmeticSticker);
        }

        // DELETE: api/CosmeticSticker/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCosmeticSticker(int id)
        {
            var cosmeticSticker = await _context.CosmeticStickers.FindAsync(id);
            if (cosmeticSticker == null)
            {
                return NotFound();
            }

            _context.CosmeticStickers.Remove(cosmeticSticker);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CosmeticStickerExists(int id)
        {
            return _context.CosmeticStickers.Any(e => e.Id == id);
        }
    }
}
