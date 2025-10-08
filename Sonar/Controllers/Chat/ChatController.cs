using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.Chat;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly SonarContext _context;

    public ChatController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Chat
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entities.Models.Chat.Chat>>> GetChats()
    {
        return await _context.Chats.ToListAsync();
    }

    // GET: api/Chat/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Entities.Models.Chat.Chat>> GetChat(int id)
    {
        Entities.Models.Chat.Chat? chat = await _context.Chats.FindAsync(id);

        if (chat == null) return NotFound();

        return chat;
    }

    // PUT: api/Chat/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutChat(int id, Entities.Models.Chat.Chat chat)
    {
        if (id != chat.Id) return BadRequest();

        _context.Entry(chat).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ChatExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Chat
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Entities.Models.Chat.Chat>> PostChat(Entities.Models.Chat.Chat chat)
    {
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetChat", new { id = chat.Id }, chat);
    }

    // DELETE: api/Chat/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChat(int id)
    {
        Entities.Models.Chat.Chat? chat = await _context.Chats.FindAsync(id);
        if (chat == null) return NotFound();

        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ChatExists(int id)
    {
        return _context.Chats.Any(e => e.Id == id);
    }
}
