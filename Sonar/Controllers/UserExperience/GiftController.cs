using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class GiftController : ControllerBase
{
    private readonly SonarContext _context;

    public GiftController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Gift
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Gift>>> GetGifts()
    {
        return await _context.Gifts.ToListAsync();
    }

    // GET: api/Gift/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Gift>> GetGift(int id)
    {
        Gift? gift = await _context.Gifts.FindAsync(id);

        if (gift == null) return NotFound();

        return gift;
    }

    // PUT: api/Gift/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGift(int id, Gift gift)
    {
        if (id != gift.Id) return BadRequest();

        _context.Entry(gift).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GiftExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Gift
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
        Gift? gift = await _context.Gifts.FindAsync(id);
        if (gift == null) return NotFound();

        _context.Gifts.Remove(gift);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool GiftExists(int id)
    {
        return _context.Gifts.Any(e => e.Id == id);
    }
}