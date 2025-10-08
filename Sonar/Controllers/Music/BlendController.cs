using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers.Music;

[Route("api/[controller]")]
[ApiController]
public class BlendController : ControllerBase
{
    // GET: api/<BlendController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET api/<BlendController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<BlendController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<BlendController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<BlendController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
