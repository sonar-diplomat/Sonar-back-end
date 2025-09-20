using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.ClientSettings
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaybackQualityController : ControllerBase
    {
        private readonly SonarContext _context;

        public PlaybackQualityController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/PlaybackQuality
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaybackQuality>>> GetPlaybackQualities()
        {
            return await _context.PlaybackQualities.ToListAsync();
        }

        // GET: api/PlaybackQuality/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlaybackQuality>> GetPlaybackQuality(int id)
        {
            var playbackQuality = await _context.PlaybackQualities.FindAsync(id);

            if (playbackQuality == null)
            {
                return NotFound();
            }

            return playbackQuality;
        }

        // PUT: api/PlaybackQuality/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaybackQuality(int id, PlaybackQuality playbackQuality)
        {
            if (id != playbackQuality.Id)
            {
                return BadRequest();
            }

            _context.Entry(playbackQuality).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaybackQualityExists(id))
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

        // POST: api/PlaybackQuality
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlaybackQuality>> PostPlaybackQuality(PlaybackQuality playbackQuality)
        {
            _context.PlaybackQualities.Add(playbackQuality);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlaybackQuality", new { id = playbackQuality.Id }, playbackQuality);
        }

        // DELETE: api/PlaybackQuality/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaybackQuality(int id)
        {
            var playbackQuality = await _context.PlaybackQualities.FindAsync(id);
            if (playbackQuality == null)
            {
                return NotFound();
            }

            _context.PlaybackQualities.Remove(playbackQuality);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaybackQualityExists(int id)
        {
            return _context.PlaybackQualities.Any(e => e.Id == id);
        }
    }
}
