using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RollenController : ControllerBase
{
    private readonly ProjectContext _context;

    public RollenController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/rollen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rollen>>> GetRollen()
    {
        return await _context.Rollens.ToListAsync();
    }

    // GET: api/rollen/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Rollen>> GetRol(int id)
    {
        var rol = await _context.Rollens.FindAsync(id);
        if (rol == null) return NotFound();
        return rol;
    }

    // POST: api/rollen
    [HttpPost]
    public async Task<ActionResult<Rollen>> PostRol(Rollen rol)
    {
        _context.Rollens.Add(rol);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRol), new { id = rol.Id }, rol);
    }

    // PUT: api/rollen/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRol(int id, Rollen rol)
    {
        if (id != rol.Id) return BadRequest();
        _context.Entry(rol).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Rollens.Any(e => e.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/rollen/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRol(int id)
    {
        var rol = await _context.Rollens.FindAsync(id);
        if (rol == null) return NotFound();
        _context.Rollens.Remove(rol);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/rollen/gebruiker/5
    [HttpGet("gebruiker/{gebruikerId}")]
    public async Task<ActionResult<IEnumerable<Rollen>>> GetRollenVanGebruiker(int gebruikerId)
    {
        var rolIds = await _context.Roltoewijzingens
            .Where(r => r.GebruikerId == gebruikerId)
            .Select(r => r.RolId)
            .ToListAsync();

        return await _context.Rollens
            .Where(r => rolIds.Contains(r.Id))
            .ToListAsync();
    }
}
