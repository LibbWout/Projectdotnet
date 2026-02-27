using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.project;

namespace project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductenController : ControllerBase
{
    private readonly ProjectContext _context;

    public ProductenController(ProjectContext context)
    {
        _context = context;
    }

    // GET: api/producten
    // Geeft alle producten terug met hun meest recente details
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetProducten()
    {
        var producten = await _context.Productens.ToListAsync();
        var result = new List<object>();

        foreach (var product in producten)
        {
            var recenteDetails = await _context.Productdetails
                .Where(pd => pd.ProductId == product.Id)
                .OrderByDescending(pd => pd.Tijdstip)
                .FirstOrDefaultAsync();

            result.Add(new
            {
                product.Id,
                Details = recenteDetails
            });
        }

        return result;
    }

    // GET: api/producten/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetProduct(int id)
    {
        var product = await _context.Productens.FindAsync(id);
        if (product == null) return NotFound();

        var recenteDetails = await _context.Productdetails
            .Where(pd => pd.ProductId == id)
            .OrderByDescending(pd => pd.Tijdstip)
            .FirstOrDefaultAsync();

        return new { product.Id, Details = recenteDetails };
    }

    // GET: api/producten/type/drank
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<object>>> GetProductenByType(string type)
    {
        var recenteDetails = await _context.Productdetails
            .Where(pd => pd.Producttype == type)
            .GroupBy(pd => pd.ProductId)
            .Select(g => g.OrderByDescending(pd => pd.Tijdstip).First())
            .ToListAsync();

        return recenteDetails.Select(pd => new
        {
            pd.ProductId,
            pd.Naam,
            pd.Prijs,
            pd.Producttype,
            pd.Tijdstip
        }).ToList<object>();
    }

    // POST: api/producten
    [HttpPost]
    public async Task<ActionResult<object>> PostProduct(NieuwProductRequest request)
    {
        var product = new Producten();
        _context.Productens.Add(product);
        await _context.SaveChangesAsync();

        var details = new Productdetail
        {
            ProductId = product.Id,
            Tijdstip = DateTime.Now,
            Naam = request.Naam,
            Prijs = request.Prijs,
            Producttype = request.Producttype
        };

        _context.Productdetails.Add(details);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id },
            new { product.Id, Details = details });
    }

    // PUT: api/producten/5/prijs
    // Voegt nieuwe productdetails toe (historiek bewaren)
    [HttpPut("{id}/details")]
    public async Task<IActionResult> UpdateProductDetails(int id, UpdateProductRequest request)
    {
        var product = await _context.Productens.FindAsync(id);
        if (product == null) return NotFound();

        var details = new Productdetail
        {
            ProductId = id,
            Tijdstip = DateTime.Now,
            Naam = request.Naam,
            Prijs = request.Prijs,
            Producttype = request.Producttype
        };

        _context.Productdetails.Add(details);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/producten/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Productens.FindAsync(id);
        if (product == null) return NotFound();

        var details = _context.Productdetails.Where(pd => pd.ProductId == id);
        _context.Productdetails.RemoveRange(details);
        _context.Productens.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class NieuwProductRequest
{
    public string Naam { get; set; } = null!;
    public float Prijs { get; set; }
    public string Producttype { get; set; } = null!;
}

public class UpdateProductRequest
{
    public string Naam { get; set; } = null!;
    public float Prijs { get; set; }
    public string Producttype { get; set; } = null!;
}
