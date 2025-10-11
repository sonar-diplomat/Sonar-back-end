using Entities.Models.Distribution;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.Distribution;

[Route("api/[controller]")]
[ApiController]
public class ArtistController : ControllerBase
{
    private readonly SonarContext _context;

    public ArtistController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Artist
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Artist>>> GetArtists()
    {
        return await _context.Artists.ToListAsync();
    }

    // GET: api/Artist/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Artist>> GetArtist(int id)
    {
        Artist? artist = await _context.Artists.FindAsync(id);

        if (artist == null) return NotFound();

        return artist;
    }

    // PUT: api/Artist/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutArtist(int id, Artist artist)
    {
        if (id != artist.Id) return BadRequest();

        _context.Entry(artist).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ArtistExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Artist
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Artist>> PostArtist(Artist artist)
    {
        _context.Artists.Add(artist);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetArtist", new { id = artist.Id }, artist);
    }

    // DELETE: api/Artist/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArtist(int id)
    {
        Artist? artist = await _context.Artists.FindAsync(id);
        if (artist == null) return NotFound();

        _context.Artists.Remove(artist);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ArtistExists(int id)
    {
        return _context.Artists.Any(e => e.Id == id);
    }
}
