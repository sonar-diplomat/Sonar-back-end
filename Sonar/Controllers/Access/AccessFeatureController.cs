using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessFeatureController(UserManager<User> userManager) : BaseController(userManager)
{
    // GET: api/Access
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessFeature>>> GetAccessFeature()
    {
        throw new NotImplementedException();
    }

    // GET: api/Access/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AccessFeature>> GetAccessFeature(int id)
    {
        throw new NotImplementedException();
    }

    // PUT: api/Access/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAccessFeature(int id, AccessFeature accessFeature)
    {
        throw new NotImplementedException();
    }

    // POST: api/Access
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<AccessFeature>> PostAccessFeature(AccessFeature accessFeature)
    {
        throw new NotImplementedException();
    }

    // DELETE: api/Access/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccessFeature(int id)
    {
        throw new NotImplementedException();
    }

    private bool AccessFeatureExists(int id)
    {
        throw new NotImplementedException();
    }
}
