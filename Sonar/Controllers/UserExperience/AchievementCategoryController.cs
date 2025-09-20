using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.UserExperience
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementCategoryController : ControllerBase
    {
        private readonly SonarContext _context;

        public AchievementCategoryController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/AchievementCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AchievementCategory>>> GetAchievementCategories()
        {
            return await _context.AchievementCategories.ToListAsync();
        }

        // GET: api/AchievementCategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AchievementCategory>> GetAchievementCategory(int id)
        {
            var achievementCategory = await _context.AchievementCategories.FindAsync(id);

            if (achievementCategory == null)
            {
                return NotFound();
            }

            return achievementCategory;
        }

        // PUT: api/AchievementCategory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAchievementCategory(int id, AchievementCategory achievementCategory)
        {
            if (id != achievementCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(achievementCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AchievementCategoryExists(id))
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

        // POST: api/AchievementCategory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AchievementCategory>> PostAchievementCategory(AchievementCategory achievementCategory)
        {
            _context.AchievementCategories.Add(achievementCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAchievementCategory", new { id = achievementCategory.Id }, achievementCategory);
        }

        // DELETE: api/AchievementCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAchievementCategory(int id)
        {
            var achievementCategory = await _context.AchievementCategories.FindAsync(id);
            if (achievementCategory == null)
            {
                return NotFound();
            }

            _context.AchievementCategories.Remove(achievementCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AchievementCategoryExists(int id)
        {
            return _context.AchievementCategories.Any(e => e.Id == id);
        }
    }
}
