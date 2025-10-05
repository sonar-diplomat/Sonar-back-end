using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class CosmeticItemController : ControllerBase
{
    private readonly SonarContext _context;

    public CosmeticItemController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/CosmeticItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CosmeticItem>>> GetCosmeticItems()
    {
        return await _context.CosmeticItems.ToListAsync();
    }

    // GET: api/CosmeticItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CosmeticItem>> GetCosmeticItem(int id)
    {
        CosmeticItem? cosmeticItem = await _context.CosmeticItems.FindAsync(id);

        if (cosmeticItem == null) return NotFound();

        return cosmeticItem;
    }

    // PUT: api/CosmeticItem/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCosmeticItem(int id, CosmeticItem cosmeticItem)
    {
        if (id != cosmeticItem.Id) return BadRequest();

        _context.Entry(cosmeticItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CosmeticItemExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/CosmeticItem
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CosmeticItem>> PostCosmeticItem(CosmeticItem cosmeticItem)
    {
        _context.CosmeticItems.Add(cosmeticItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCosmeticItem", new { id = cosmeticItem.Id }, cosmeticItem);
    }

    // DELETE: api/CosmeticItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCosmeticItem(int id)
    {
        CosmeticItem? cosmeticItem = await _context.CosmeticItems.FindAsync(id);
        if (cosmeticItem == null) return NotFound();

        _context.CosmeticItems.Remove(cosmeticItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CosmeticItemExists(int id)
    {
        return _context.CosmeticItems.Any(e => e.Id == id);
    }
}