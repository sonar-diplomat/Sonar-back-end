using Entities.Models.UserExperience;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.UserExperience;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionFeatureController : ControllerBase
{
    private readonly SonarContext _context;

    public SubscriptionFeatureController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/SubscriptionFeature
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionFeature>>> GetSubscriptionFeatures()
    {
        return await _context.SubscriptionFeatures.ToListAsync();
    }

    // GET: api/SubscriptionFeature/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SubscriptionFeature>> GetSubscriptionFeature(int id)
    {
        SubscriptionFeature? subscriptionFeature = await _context.SubscriptionFeatures.FindAsync(id);

        if (subscriptionFeature == null) return NotFound();

        return subscriptionFeature;
    }

    // PUT: api/SubscriptionFeature/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSubscriptionFeature(int id, SubscriptionFeature subscriptionFeature)
    {
        if (id != subscriptionFeature.Id) return BadRequest();

        _context.Entry(subscriptionFeature).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SubscriptionFeatureExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/SubscriptionFeature
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<SubscriptionFeature>> PostSubscriptionFeature(
        SubscriptionFeature subscriptionFeature)
    {
        _context.SubscriptionFeatures.Add(subscriptionFeature);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSubscriptionFeature", new { id = subscriptionFeature.Id }, subscriptionFeature);
    }

    // DELETE: api/SubscriptionFeature/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubscriptionFeature(int id)
    {
        SubscriptionFeature? subscriptionFeature = await _context.SubscriptionFeatures.FindAsync(id);
        if (subscriptionFeature == null) return NotFound();

        _context.SubscriptionFeatures.Remove(subscriptionFeature);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SubscriptionFeatureExists(int id)
    {
        return _context.SubscriptionFeatures.Any(e => e.Id == id);
    }
}