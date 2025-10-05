using Entities.Models.Access;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessFeatureController : ControllerBase
{
    private readonly SonarContext _context;

    public AccessFeatureController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Access
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessFeature>>> GetAccessFeature()
    {
        return await _context.AccessFeatures.ToListAsync();
    }

    // GET: api/Access/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AccessFeature>> GetAccessFeature(int id)
    {
        AccessFeature? accessFeature = await _context.AccessFeatures.FindAsync(id);

        if (accessFeature == null) return NotFound();

        return accessFeature;
    }

    // PUT: api/Access/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAccessFeature(int id, AccessFeature accessFeature)
    {
        if (id != accessFeature.Id) return BadRequest();

        _context.Entry(accessFeature).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AccessFeatureExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Access
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<AccessFeature>> PostAccessFeature(AccessFeature accessFeature)
    {
        _context.AccessFeatures.Add(accessFeature);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAccessFeature", new { id = accessFeature.Id }, accessFeature);
    }

    // DELETE: api/Access/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccessFeature(int id)
    {
        AccessFeature? accessFeature = await _context.AccessFeatures.FindAsync(id);
        if (accessFeature == null) return NotFound();

        _context.AccessFeatures.Remove(accessFeature);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AccessFeatureExists(int id)
    {
        return _context.AccessFeatures.Any(e => e.Id == id);
    }
}