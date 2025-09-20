using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSessionController : ControllerBase
    {
        private readonly SonarContext _context;

        public UserSessionController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/UserSession
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSession>>> GetUserSessions()
        {
            return await _context.UserSessions.ToListAsync();
        }

        // GET: api/UserSession/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSession>> GetUserSession(int id)
        {
            var userSession = await _context.UserSessions.FindAsync(id);

            if (userSession == null)
            {
                return NotFound();
            }

            return userSession;
        }

        // PUT: api/UserSession/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserSession(int id, UserSession userSession)
        {
            if (id != userSession.Id)
            {
                return BadRequest();
            }

            _context.Entry(userSession).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSessionExists(id))
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

        // POST: api/UserSession
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserSession>> PostUserSession(UserSession userSession)
        {
            _context.UserSessions.Add(userSession);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserSession", new { id = userSession.Id }, userSession);
        }

        // DELETE: api/UserSession/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSession(int id)
        {
            var userSession = await _context.UserSessions.FindAsync(id);
            if (userSession == null)
            {
                return NotFound();
            }

            _context.UserSessions.Remove(userSession);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserSessionExists(int id)
        {
            return _context.UserSessions.Any(e => e.Id == id);
        }
    }
}
