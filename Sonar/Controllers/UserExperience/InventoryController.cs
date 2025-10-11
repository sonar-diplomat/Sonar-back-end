using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly SonarContext _context;

    public InventoryController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Inventory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories()
    {
        return await _context.Inventories.ToListAsync();
    }

    // GET: api/Inventory/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Inventory>> GetInventory(int id)
    {
        Inventory? inventory = await _context.Inventories.FindAsync(id);

        if (inventory == null) return NotFound();

        return inventory;
    }

    // PUT: api/Inventory/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInventory(int id, Inventory inventory)
    {
        if (id != inventory.Id) return BadRequest();

        _context.Entry(inventory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InventoryExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Inventory
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
    {
        _context.Inventories.Add(inventory);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetInventory", new { id = inventory.Id }, inventory);
    }

    // DELETE: api/Inventory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        Inventory? inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null) return NotFound();

        _context.Inventories.Remove(inventory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool InventoryExists(int id)
    {
        return _context.Inventories.Any(e => e.Id == id);
    }
}
