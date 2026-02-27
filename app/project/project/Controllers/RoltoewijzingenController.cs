using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoltoewijzingenController : ControllerBase
{
    private readonly ProjectContext _context;

    public RoltoewijzingenController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/roltoewijzingen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Roltoewijzingen>>> GetRoltoewijzingen()
    {
        return await _context.Roltoewijzingens.ToListAsync();
    }

    // GET: api/roltoewijzingen/gebruiker/5
    [HttpGet("gebruiker/{gebruikerId}")]
    public async Task<ActionResult<IEnumerable<Roltoewijzingen>>> GetRoltoewijzingenVanGebruiker(int gebruikerId)
    {
        return await _context.Roltoewijzingens
            .Where(r => r.GebruikerId == gebruikerId)
            .ToListAsync();
    }

    // POST: api/roltoewijzingen
    [HttpPost]
    public async Task<ActionResult<Roltoewijzingen>> PostRoltoewijzing(Roltoewijzingen roltoewijzing)
    {
        var bestaand = await _context.Roltoewijzingens
            .FindAsync(roltoewijzing.GebruikerId, roltoewijzing.RolId);
        if (bestaand != null) return Conflict("Gebruiker heeft deze rol al.");

        _context.Roltoewijzingens.Add(roltoewijzing);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRoltoewijzingenVanGebruiker),
            new { gebruikerId = roltoewijzing.GebruikerId }, roltoewijzing);
    }

    // DELETE: api/roltoewijzingen/gebruiker/5/rol/2
    [HttpDelete("gebruiker/{gebruikerId}/rol/{rolId}")]
    public async Task<IActionResult> DeleteRoltoewijzing(int gebruikerId, int rolId)
    {
        var roltoewijzing = await _context.Roltoewijzingens.FindAsync(gebruikerId, rolId);
        if (roltoewijzing == null) return NotFound();
        _context.Roltoewijzingens.Remove(roltoewijzing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
