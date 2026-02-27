using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatistiekenController : ControllerBase
{
    private readonly ProjectContext _context;

    public StatistiekenController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/statistieken/meest-besteld/drank
    [HttpGet("meest-besteld/{type}")]
    public async Task<ActionResult<object>> GetMeestBesteld(string type)
    {
        var productIds = await _context.Productdetails
            .Where(pd => pd.Producttype == type)
            .Select(pd => pd.ProductId)
            .Distinct()
            .ToListAsync();

        var meestBesteld = await _context.Bestellijnens
            .Where(bl => productIds.Contains(bl.ProductId))
            .GroupBy(bl => bl.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotaalBesteld = g.Sum(bl => bl.Hoeveelheid)
            })
            .OrderByDescending(x => x.TotaalBesteld)
            .FirstOrDefaultAsync();

        if (meestBesteld == null) return NotFound();

        var details = await _context.Productdetails
            .Where(pd => pd.ProductId == meestBesteld.ProductId)
            .OrderByDescending(pd => pd.Tijdstip)
            .FirstOrDefaultAsync();

        return new
        {
            meestBesteld.ProductId,
            ProductNaam = details?.Naam,
            meestBesteld.TotaalBesteld
        };
    }

    // GET: api/statistieken/minst-besteld/drank
    [HttpGet("minst-besteld/{type}")]
    public async Task<ActionResult<object>> GetMinstBesteld(string type)
    {
        var productIds = await _context.Productdetails
            .Where(pd => pd.Producttype == type)
            .Select(pd => pd.ProductId)
            .Distinct()
            .ToListAsync();

        var minstBesteld = await _context.Bestellijnens
            .Where(bl => productIds.Contains(bl.ProductId))
            .GroupBy(bl => bl.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotaalBesteld = g.Sum(bl => bl.Hoeveelheid)
            })
            .OrderBy(x => x.TotaalBesteld)
            .FirstOrDefaultAsync();

        if (minstBesteld == null) return NotFound();

        var details = await _context.Productdetails
            .Where(pd => pd.ProductId == minstBesteld.ProductId)
            .OrderByDescending(pd => pd.Tijdstip)
            .FirstOrDefaultAsync();

        return new
        {
            minstBesteld.ProductId,
            ProductNaam = details?.Naam,
            minstBesteld.TotaalBesteld
        };
    }

    // GET: api/statistieken/tafel-meeste-uitgaven/drank
    [HttpGet("tafel-meeste-uitgaven/{type}")]
    public async Task<ActionResult<object>> GetTafelMeesteUitgaven(string type)
    {
        var productIds = await _context.Productdetails
            .Where(pd => pd.Producttype == type)
            .Select(pd => pd.ProductId)
            .Distinct()
            .ToListAsync();

        var bestellingen = await _context.Bestellingens.ToListAsync();
        var tafelUitgaven = new Dictionary<int, float>();

        foreach (var bestelling in bestellingen)
        {
            // Zoek tafeltoewijzing op tijdstip van bestelling
            var tafeltoewijzing = await _context.Tafeltoewijzingens
                .Where(tt => tt.GebruikerId == bestelling.GebruikerId
                    && tt.TijdstipToegewezen <= bestelling.TijdstipBesteld)
                .OrderByDescending(tt => tt.TijdstipToegewezen)
                .FirstOrDefaultAsync();

            if (tafeltoewijzing == null) continue;

            var lijnen = await _context.Bestellijnens
                .Where(bl => bl.BestellingId == bestelling.Id
                    && productIds.Contains(bl.ProductId))
                .ToListAsync();

            foreach (var lijn in lijnen)
            {
                var details = await _context.Productdetails
                    .Where(pd => pd.ProductId == lijn.ProductId)
                    .OrderByDescending(pd => pd.Tijdstip)
                    .FirstOrDefaultAsync();

                float subtotaal = (details?.Prijs ?? 0) * lijn.Hoeveelheid;

                if (!tafelUitgaven.ContainsKey(tafeltoewijzing.TafelId))
                    tafelUitgaven[tafeltoewijzing.TafelId] = 0;

                tafelUitgaven[tafeltoewijzing.TafelId] += subtotaal;
            }
        }

        if (!tafelUitgaven.Any()) return NotFound();

        var topTafelId = tafelUitgaven.OrderByDescending(t => t.Value).First();
        var tafel = await _context.Tafels.FindAsync(topTafelId.Key);

        return new
        {
            TafelId = topTafelId.Key,
            TafelNaam = tafel?.Naam,
            TotaleUitgaven = topTafelId.Value
        };
    }
}
