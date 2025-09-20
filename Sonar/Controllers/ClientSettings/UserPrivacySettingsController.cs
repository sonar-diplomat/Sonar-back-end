using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.ClientSettings
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPrivacySettingsController : ControllerBase
    {
        private readonly SonarContext _context;

        public UserPrivacySettingsController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/UserPrivacySettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPrivacySettings>>> GetUserPrivacySettings()
        {
            return await _context.UserPrivacySettings.ToListAsync();
        }

        // GET: api/UserPrivacySettings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPrivacySettings>> GetUserPrivacySettings(int id)
        {
            var userPrivacySettings = await _context.UserPrivacySettings.FindAsync(id);

            if (userPrivacySettings == null)
            {
                return NotFound();
            }

            return userPrivacySettings;
        }

        // PUT: api/UserPrivacySettings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserPrivacySettings(int id, UserPrivacySettings userPrivacySettings)
        {
            if (id != userPrivacySettings.Id)
            {
                return BadRequest();
            }

            _context.Entry(userPrivacySettings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserPrivacySettingsExists(id))
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

        // POST: api/UserPrivacySettings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserPrivacySettings>> PostUserPrivacySettings(UserPrivacySettings userPrivacySettings)
        {
            _context.UserPrivacySettings.Add(userPrivacySettings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserPrivacySettings", new { id = userPrivacySettings.Id }, userPrivacySettings);
        }

        // DELETE: api/UserPrivacySettings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPrivacySettings(int id)
        {
            var userPrivacySettings = await _context.UserPrivacySettings.FindAsync(id);
            if (userPrivacySettings == null)
            {
                return NotFound();
            }

            _context.UserPrivacySettings.Remove(userPrivacySettings);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserPrivacySettingsExists(int id)
        {
            return _context.UserPrivacySettings.Any(e => e.Id == id);
        }
    }
}
