using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Infrastructure.Data;

namespace Sonar.Controllers.File
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileTypeController : ControllerBase
    {
        private readonly SonarContext _context;

        public FileTypeController(SonarContext context)
        {
            _context = context;
        }

        // GET: api/FileType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileType>>> GetFileTypes()
        {
            return await _context.FileTypes.ToListAsync();
        }

        // GET: api/FileType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FileType>> GetFileType(int id)
        {
            var fileType = await _context.FileTypes.FindAsync(id);

            if (fileType == null)
            {
                return NotFound();
            }

            return fileType;
        }

        // PUT: api/FileType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileType(int id, FileType fileType)
        {
            if (id != fileType.Id)
            {
                return BadRequest();
            }

            _context.Entry(fileType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileTypeExists(id))
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

        // POST: api/FileType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FileType>> PostFileType(FileType fileType)
        {
            _context.FileTypes.Add(fileType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFileType", new { id = fileType.Id }, fileType);
        }

        // DELETE: api/FileType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileType(int id)
        {
            var fileType = await _context.FileTypes.FindAsync(id);
            if (fileType == null)
            {
                return NotFound();
            }

            _context.FileTypes.Remove(fileType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileTypeExists(int id)
        {
            return _context.FileTypes.Any(e => e.Id == id);
        }
    }
}
