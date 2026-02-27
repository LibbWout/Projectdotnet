using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BestellingenController : ControllerBase
{
    private readonly ProjectContext _context;

    public BestellingenController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/bestellingen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetBestellingen()
    {
        var bestellingen = await _context.Bestellingens.ToListAsync();
        var result = new List<object>();

        foreach (var bestelling in bestellingen)
        {
            var lijnen = await _context.Bestellijnens
                .Where(bl => bl.BestellingId == bestelling.Id)
                .ToListAsync();

            var gebruiker = await _context.Gebruikers.FindAsync(bestelling.GebruikerId);

            result.Add(new
            {
                bestelling.Id,
                bestelling.GebruikerId,
                GebruikerNaam = gebruiker?.Naam,
                bestelling.TijdstipBesteld,
                bestelling.Status,
                Lijnen = lijnen
            });
        }

        return result;
    }

    // GET: api/bestellingen/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetBestelling(int id)
    {
        var bestelling = await _context.Bestellingens.FindAsync(id);
        if (bestelling == null) return NotFound();

        var lijnen = await _context.Bestellijnens
            .Where(bl => bl.BestellingId == id)
            .ToListAsync();

        var gebruiker = await _context.Gebruikers.FindAsync(bestelling.GebruikerId);

        // Bereken totaalprijs
        float totaal = 0;
        var lijnenMetPrijs = new List<object>();
        foreach (var lijn in lijnen)
        {
            var details = await _context.Productdetails
                .Where(pd => pd.ProductId == lijn.ProductId)
                .OrderByDescending(pd => pd.Tijdstip)
                .FirstOrDefaultAsync();

            float subtotaal = (details?.Prijs ?? 0) * lijn.Hoeveelheid;
            totaal += subtotaal;

            lijnenMetPrijs.Add(new
            {
                lijn.ProductId,
                ProductNaam = details?.Naam,
                EenheidsPrijs = details?.Prijs,
                lijn.Hoeveelheid,
                Subtotaal = subtotaal
            });
        }

        return new
        {
            bestelling.Id,
            bestelling.GebruikerId,
            GebruikerNaam = gebruiker?.Naam,
            bestelling.TijdstipBesteld,
            bestelling.Status,
            Lijnen = lijnenMetPrijs,
            Totaal = totaal
        };
    }

    // GET: api/bestellingen/gebruiker/5
    [HttpGet("gebruiker/{gebruikerId}")]
    public async Task<ActionResult<IEnumerable<Bestellingen>>> GetBestellingenVanGebruiker(int gebruikerId)
    {
        return await _context.Bestellingens
            .Where(b => b.GebruikerId == gebruikerId)
            .OrderByDescending(b => b.TijdstipBesteld)
            .ToListAsync();
    }

    // GET: api/bestellingen/status/wachtrij
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<Bestellingen>>> GetBestellingenByStatus(string status)
    {
        return await _context.Bestellingens
            .Where(b => b.Status == status)
            .OrderBy(b => b.TijdstipBesteld)
            .ToListAsync();
    }

    // GET: api/bestellingen/type/drank
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<object>>> GetBestellingenByProductType(string type)
    {
        var productIds = await _context.Productdetails
            .Where(pd => pd.Producttype == type)
            .Select(pd => pd.ProductId)
            .Distinct()
            .ToListAsync();

        var bestellijnen = await _context.Bestellijnens
            .Where(bl => productIds.Contains(bl.ProductId))
            .ToListAsync();

        var bestellingIds = bestellijnen.Select(bl => bl.BestellingId).Distinct().ToList();

        var bestellingen = await _context.Bestellingens
            .Where(b => bestellingIds.Contains(b.Id))
            .ToListAsync();

        var result = new List<object>();
        foreach (var bestelling in bestellingen)
        {
            var lijnen = bestellijnen.Where(bl => bl.BestellingId == bestelling.Id).ToList();
            result.Add(new
            {
                bestelling.Id,
                bestelling.GebruikerId,
                bestelling.TijdstipBesteld,
                bestelling.Status,
                Lijnen = lijnen
            });
        }

        return result;
    }

    // POST: api/bestellingen
    [HttpPost]
    public async Task<ActionResult<Bestellingen>> PostBestelling(NieuwBestellingRequest request)
    {
        var bestelling = new Bestellingen
        {
            GebruikerId = request.GebruikerId,
            TijdstipBesteld = DateTime.Now,
            Status = "wachtrij"
        };

        _context.Bestellingens.Add(bestelling);
        await _context.SaveChangesAsync();

        foreach (var lijn in request.Lijnen)
        {
            _context.Bestellijnens.Add(new Bestellijnen
            {
                BestellingId = bestelling.Id,
                ProductId = lijn.ProductId,
                Hoeveelheid = lijn.Hoeveelheid
            });
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBestelling), new { id = bestelling.Id }, bestelling);
    }

    // PUT: api/bestellingen/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusRequest request)
    {
        var bestelling = await _context.Bestellingens.FindAsync(id);
        if (bestelling == null) return NotFound();

        bestelling.Status = request.Status;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/bestellingen/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBestelling(int id)
    {
        var bestelling = await _context.Bestellingens.FindAsync(id);
        if (bestelling == null) return NotFound();

        var lijnen = _context.Bestellijnens.Where(bl => bl.BestellingId == id);
        _context.Bestellijnens.RemoveRange(lijnen);
        _context.Bestellingens.Remove(bestelling);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class NieuwBestellingRequest
{
    public int GebruikerId { get; set; }
    public List<BestellijnRequest> Lijnen { get; set; } = new();
}

public class BestellijnRequest
{
    public int ProductId { get; set; }
    public int Hoeveelheid { get; set; }
}

public class StatusRequest
{
    public string Status { get; set; } = null!;
}
