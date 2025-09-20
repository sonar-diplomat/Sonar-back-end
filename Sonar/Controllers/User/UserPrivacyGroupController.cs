using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPrivacyGroupController : ControllerBase
    {
        private readonly SonarContext _context;

        public UserPrivacyGroupController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/UserPrivacyGroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPrivacyGroup>>> GetUserPrivacyGroups()
        {
            return await _context.UserPrivacyGroups.ToListAsync();
        }

        // GET: api/UserPrivacyGroup/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPrivacyGroup>> GetUserPrivacyGroup(int id)
        {
            var userPrivacyGroup = await _context.UserPrivacyGroups.FindAsync(id);

            if (userPrivacyGroup == null)
            {
                return NotFound();
            }

            return userPrivacyGroup;
        }

        // PUT: api/UserPrivacyGroup/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserPrivacyGroup(int id, UserPrivacyGroup userPrivacyGroup)
        {
            if (id != userPrivacyGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(userPrivacyGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserPrivacyGroupExists(id))
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

        // POST: api/UserPrivacyGroup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserPrivacyGroup>> PostUserPrivacyGroup(UserPrivacyGroup userPrivacyGroup)
        {
            _context.UserPrivacyGroups.Add(userPrivacyGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserPrivacyGroup", new { id = userPrivacyGroup.Id }, userPrivacyGroup);
        }

        // DELETE: api/UserPrivacyGroup/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPrivacyGroup(int id)
        {
            var userPrivacyGroup = await _context.UserPrivacyGroups.FindAsync(id);
            if (userPrivacyGroup == null)
            {
                return NotFound();
            }

            _context.UserPrivacyGroups.Remove(userPrivacyGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserPrivacyGroupExists(int id)
        {
            return _context.UserPrivacyGroups.Any(e => e.Id == id);
        }
    }
}
