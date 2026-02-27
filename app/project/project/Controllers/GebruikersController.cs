using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GebruikersController : ControllerBase
{
    private readonly ProjectContext _context;

    public GebruikersController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/gebruikers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
    {
        return await _context.Gebruikers.ToListAsync();
    }

    // GET: api/gebruikers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Gebruiker>> GetGebruiker(int id)
    {
        var gebruiker = await _context.Gebruikers.FindAsync(id);
        if (gebruiker == null) return NotFound();
        return gebruiker;
    }

    // GET: api/gebruikers/code/{code}
    [HttpGet("code/{code}")]
    public async Task<ActionResult<Gebruiker>> GetGebruikerByCode(string code)
    {
        var gebruiker = await _context.Gebruikers
            .FirstOrDefaultAsync(g => g.UniekeCode == code);
        if (gebruiker == null) return NotFound();
        return gebruiker;
    }

    // POST: api/gebruikers
    [HttpPost]
    public async Task<ActionResult<Gebruiker>> PostGebruiker(Gebruiker gebruiker)
    {
        gebruiker.UniekeCode = Guid.NewGuid().ToString();
        _context.Gebruikers.Add(gebruiker);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.Id }, gebruiker);
    }

    // PUT: api/gebruikers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGebruiker(int id, Gebruiker gebruiker)
    {
        if (id != gebruiker.Id) return BadRequest();
        _context.Entry(gebruiker).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Gebruikers.Any(e => e.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // PUT: api/gebruikers/5/activeer
    [HttpPut("{id}/activeer")]
    public async Task<IActionResult> ActiveerGebruiker(int id, [FromBody] ActiveerRequest request)
    {
        var gebruiker = await _context.Gebruikers.FindAsync(id);
        if (gebruiker == null) return NotFound();

        gebruiker.Naam = request.Naam;
        gebruiker.WachtwoordHash = request.WachtwoordHash;
        gebruiker.TijdstipGeactiveerd = DateTime.Now;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/gebruikers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGebruiker(int id)
    {
        var gebruiker = await _context.Gebruikers.FindAsync(id);
        if (gebruiker == null) return NotFound();
        _context.Gebruikers.Remove(gebruiker);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class ActiveerRequest
{
    public string Naam { get; set; } = null!;
    public string WachtwoordHash { get; set; } = null!;
}
