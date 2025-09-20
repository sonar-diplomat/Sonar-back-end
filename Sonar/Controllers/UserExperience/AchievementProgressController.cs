using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementProgressController : ControllerBase
    {
        private readonly SonarContext _context;

        public AchievementProgressController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/AchievementProgress
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AchievementProgress>>> GetAchievementProgresses()
        {
            return await _context.AchievementProgresses.ToListAsync();
        }

        // GET: api/AchievementProgress/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AchievementProgress>> GetAchievementProgress(int id)
        {
            var achievementProgress = await _context.AchievementProgresses.FindAsync(id);

            if (achievementProgress == null)
            {
                return NotFound();
            }

            return achievementProgress;
        }

        // PUT: api/AchievementProgress/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAchievementProgress(int id, AchievementProgress achievementProgress)
        {
            if (id != achievementProgress.Id)
            {
                return BadRequest();
            }

            _context.Entry(achievementProgress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AchievementProgressExists(id))
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

        // POST: api/AchievementProgress
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AchievementProgress>> PostAchievementProgress(AchievementProgress achievementProgress)
        {
            _context.AchievementProgresses.Add(achievementProgress);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAchievementProgress", new { id = achievementProgress.Id }, achievementProgress);
        }

        // DELETE: api/AchievementProgress/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAchievementProgress(int id)
        {
            var achievementProgress = await _context.AchievementProgresses.FindAsync(id);
            if (achievementProgress == null)
            {
                return NotFound();
            }

            _context.AchievementProgresses.Remove(achievementProgress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AchievementProgressExists(int id)
        {
            return _context.AchievementProgresses.Any(e => e.Id == id);
        }
    }
}
