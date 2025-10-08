using Entities.Models.Distribution;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class DistributorController : ControllerBase
{
    private readonly SonarContext _context;

    public DistributorController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Distributor
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Distributor>>> GetDistributors()
    {
        return await _context.Distributors.ToListAsync();
    }

    // GET: api/Distributor/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Distributor>> GetDistributor(int id)
    {
        Distributor? distributor = await _context.Distributors.FindAsync(id);

        if (distributor == null) return NotFound();

        return distributor;
    }

    // PUT: api/Distributor/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDistributor(int id, Distributor distributor)
    {
        if (id != distributor.Id) return BadRequest();

        _context.Entry(distributor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DistributorExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Distributor
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Distributor>> PostDistributor(Distributor distributor)
    {
        _context.Distributors.Add(distributor);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetDistributor", new { id = distributor.Id }, distributor);
    }

    // DELETE: api/Distributor/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDistributor(int id)
    {
        Distributor? distributor = await _context.Distributors.FindAsync(id);
        if (distributor == null) return NotFound();

        _context.Distributors.Remove(distributor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DistributorExists(int id)
    {
        return _context.Distributors.Any(e => e.Id == id);
    }
}
