using Entities.Models.Music;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class TrackController : ControllerBase
{
    private readonly SonarContext _context;

    public TrackController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Track
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Track>>> GetTracks()
    {
        return await _context.Tracks.ToListAsync();
    }

    // GET: api/Track/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Track>> GetTrack(int id)
    {
        Track? track = await _context.Tracks.FindAsync(id);

        if (track == null) return NotFound();

        return track;
    }

    // PUT: api/Track/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTrack(int id, Track track)
    {
        if (id != track.Id) return BadRequest();

        _context.Entry(track).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TrackExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Track
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Track>> PostTrack(Track track)
    {
        _context.Tracks.Add(track);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTrack", new { id = track.Id }, track);
    }

    // DELETE: api/Track/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrack(int id)
    {
        Track? track = await _context.Tracks.FindAsync(id);
        if (track == null) return NotFound();

        _context.Tracks.Remove(track);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TrackExists(int id)
    {
        return _context.Tracks.Any(e => e.Id == id);
    }
}
