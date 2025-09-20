using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.Chat
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageReadController : ControllerBase
    {
        private readonly SonarContext _context;

        public MessageReadController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/MessageRead
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageRead>>> GetMessageReads()
        {
            return await _context.MessageReads.ToListAsync();
        }

        // GET: api/MessageRead/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageRead>> GetMessageRead(int id)
        {
            var messageRead = await _context.MessageReads.FindAsync(id);

            if (messageRead == null)
            {
                return NotFound();
            }

            return messageRead;
        }

        // PUT: api/MessageRead/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessageRead(int id, MessageRead messageRead)
        {
            if (id != messageRead.Id)
            {
                return BadRequest();
            }

            _context.Entry(messageRead).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageReadExists(id))
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

        // POST: api/MessageRead
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MessageRead>> PostMessageRead(MessageRead messageRead)
        {
            _context.MessageReads.Add(messageRead);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessageRead", new { id = messageRead.Id }, messageRead);
        }

        // DELETE: api/MessageRead/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessageRead(int id)
        {
            var messageRead = await _context.MessageReads.FindAsync(id);
            if (messageRead == null)
            {
                return NotFound();
            }

            _context.MessageReads.Remove(messageRead);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageReadExists(int id)
        {
            return _context.MessageReads.Any(e => e.Id == id);
        }
    }
}
