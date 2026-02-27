using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TafelsController : ControllerBase
{
    private readonly ProjectContext _context;

    public TafelsController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/tafels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tafel>>> GetTafels()
    {
        return await _context.Tafels.ToListAsync();
    }

    // GET: api/tafels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Tafel>> GetTafel(int id)
    {
        var tafel = await _context.Tafels.FindAsync(id);
        if (tafel == null) return NotFound();
        return tafel;
    }

    // POST: api/tafels
    [HttpPost]
    public async Task<ActionResult<Tafel>> PostTafel(Tafel tafel)
    {
        _context.Tafels.Add(tafel);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTafel), new { id = tafel.Id }, tafel);
    }

    // PUT: api/tafels/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTafel(int id, Tafel tafel)
    {
        if (id != tafel.Id) return BadRequest();
        _context.Entry(tafel).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tafels.Any(e => e.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/tafels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTafel(int id)
    {
        var tafel = await _context.Tafels.FindAsync(id);
        if (tafel == null) return NotFound();
        _context.Tafels.Remove(tafel);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
