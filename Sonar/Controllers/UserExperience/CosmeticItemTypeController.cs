using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmeticItemTypeController : ControllerBase
    {
        private readonly SonarContext _context;

        public CosmeticItemTypeController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/CosmeticItemType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CosmeticItemType>>> GetCosmeticItemTypes()
        {
            return await _context.CosmeticItemTypes.ToListAsync();
        }

        // GET: api/CosmeticItemType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CosmeticItemType>> GetCosmeticItemType(int id)
        {
            var cosmeticItemType = await _context.CosmeticItemTypes.FindAsync(id);

            if (cosmeticItemType == null)
            {
                return NotFound();
            }

            return cosmeticItemType;
        }

        // PUT: api/CosmeticItemType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCosmeticItemType(int id, CosmeticItemType cosmeticItemType)
        {
            if (id != cosmeticItemType.Id)
            {
                return BadRequest();
            }

            _context.Entry(cosmeticItemType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CosmeticItemTypeExists(id))
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

        // POST: api/CosmeticItemType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CosmeticItemType>> PostCosmeticItemType(CosmeticItemType cosmeticItemType)
        {
            _context.CosmeticItemTypes.Add(cosmeticItemType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCosmeticItemType", new { id = cosmeticItemType.Id }, cosmeticItemType);
        }

        // DELETE: api/CosmeticItemType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCosmeticItemType(int id)
        {
            var cosmeticItemType = await _context.CosmeticItemTypes.FindAsync(id);
            if (cosmeticItemType == null)
            {
                return NotFound();
            }

            _context.CosmeticItemTypes.Remove(cosmeticItemType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CosmeticItemTypeExists(int id)
        {
            return _context.CosmeticItemTypes.Any(e => e.Id == id);
        }
    }
}
