using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TafeltoewijzingenController : ControllerBase
{
    private readonly ProjectContext _context;

    public TafeltoewijzingenController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/tafeltoewijzingen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tafeltoewijzingen>>> GetTafeltoewijzingen()
    {
        return await _context.Tafeltoewijzingens.ToListAsync();
    }

    // GET: api/tafeltoewijzingen/gebruiker/5
    [HttpGet("gebruiker/{gebruikerId}")]
    public async Task<ActionResult<IEnumerable<Tafeltoewijzingen>>> GetTafeltoewijzingenVanGebruiker(int gebruikerId)
    {
        return await _context.Tafeltoewijzingens
            .Where(t => t.GebruikerId == gebruikerId)
            .OrderByDescending(t => t.TijdstipToegewezen)
            .ToListAsync();
    }

    // GET: api/tafeltoewijzingen/tafel/5
    [HttpGet("tafel/{tafelId}")]
    public async Task<ActionResult<IEnumerable<Tafeltoewijzingen>>> GetTafeltoewijzingenVanTafel(int tafelId)
    {
        return await _context.Tafeltoewijzingens
            .Where(t => t.TafelId == tafelId)
            .OrderByDescending(t => t.TijdstipToegewezen)
            .ToListAsync();
    }

    // GET: api/tafeltoewijzingen/gebruiker/5/huidig
    [HttpGet("gebruiker/{gebruikerId}/huidig")]
    public async Task<ActionResult<Tafeltoewijzingen>> GetHuidigeTabeltoewijzing(int gebruikerId)
    {
        var toewijzing = await _context.Tafeltoewijzingens
            .Where(t => t.GebruikerId == gebruikerId)
            .OrderByDescending(t => t.TijdstipToegewezen)
            .FirstOrDefaultAsync();

        if (toewijzing == null) return NotFound();
        return toewijzing;
    }

    // POST: api/tafeltoewijzingen
    [HttpPost]
    public async Task<ActionResult<Tafeltoewijzingen>> PostTafeltoewijzing(Tafeltoewijzingen tafeltoewijzing)
    {
        tafeltoewijzing.TijdstipToegewezen = DateTime.Now;
        _context.Tafeltoewijzingens.Add(tafeltoewijzing);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTafeltoewijzingenVanGebruiker),
            new { gebruikerId = tafeltoewijzing.GebruikerId }, tafeltoewijzing);
    }

    // DELETE: api/tafeltoewijzingen/gebruiker/5/tafel/2
    [HttpDelete("gebruiker/{gebruikerId}/tafel/{tafelId}")]
    public async Task<IActionResult> DeleteTafeltoewijzing(int gebruikerId, int tafelId)
    {
        var toewijzingen = await _context.Tafeltoewijzingens
            .Where(t => t.GebruikerId == gebruikerId && t.TafelId == tafelId)
            .ToListAsync();

        if (!toewijzingen.Any()) return NotFound();
        _context.Tafeltoewijzingens.RemoveRange(toewijzingen);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
