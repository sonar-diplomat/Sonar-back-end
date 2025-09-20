using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStateController : ControllerBase
    {
        private readonly SonarContext _context;

        public UserStateController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/UserState
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserState>>> GetUserStates()
        {
            return await _context.UserStates.ToListAsync();
        }

        // GET: api/UserState/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserState>> GetUserState(int id)
        {
            var userState = await _context.UserStates.FindAsync(id);

            if (userState == null)
            {
                return NotFound();
            }

            return userState;
        }

        // PUT: api/UserState/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserState(int id, UserState userState)
        {
            if (id != userState.Id)
            {
                return BadRequest();
            }

            _context.Entry(userState).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserStateExists(id))
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

        // POST: api/UserState
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserState>> PostUserState(UserState userState)
        {
            _context.UserStates.Add(userState);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserState", new { id = userState.Id }, userState);
        }

        // DELETE: api/UserState/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserState(int id)
        {
            var userState = await _context.UserStates.FindAsync(id);
            if (userState == null)
            {
                return NotFound();
            }

            _context.UserStates.Remove(userState);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserStateExists(int id)
        {
            return _context.UserStates.Any(e => e.Id == id);
        }
    }
}
