using Entities.Models.Library;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Controllers.Library;

[Route("api/[controller]")]
[ApiController]
public class FolderController : ControllerBase
{
    private readonly SonarContext _context;

    public FolderController(SonarContext context)
    {
        _context = context;
    }

    // GET: api/Folder
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Folder>>> GetFolders()
    {
        return await _context.Folders.ToListAsync();
    }

    // GET: api/Folder/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Folder>> GetFolder(int id)
    {
        Folder? folder = await _context.Folders.FindAsync(id);

        if (folder == null) return NotFound();

        return folder;
    }

    // PUT: api/Folder/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFolder(int id, Folder folder)
    {
        if (id != folder.Id) return BadRequest();

        _context.Entry(folder).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FolderExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Folder
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Folder>> PostFolder(Folder folder)
    {
        _context.Folders.Add(folder);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetFolder", new { id = folder.Id }, folder);
    }

    // DELETE: api/Folder/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFolder(int id)
    {
        Folder? folder = await _context.Folders.FindAsync(id);
        if (folder == null) return NotFound();

        _context.Folders.Remove(folder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool FolderExists(int id)
    {
        return _context.Folders.Any(e => e.Id == id);
    }
}
