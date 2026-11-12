using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Api.Controllers;

[ApiController]
[Route("api/pricelists")]
public class PriceListsController : ControllerBase
{
    private readonly QtkdDbContext _context;

    public PriceListsController(QtkdDbContext context)
    {
        _context = context;
    }

    // GET: api/pricelists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriceList>>> GetPriceLists()
    {
        var priceLists = await _context.PriceLists
            .Include(p => p.PriceItems)
            .ToListAsync();

        return Ok(priceLists);
    }

    // GET: api/pricelists/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PriceList>> GetPriceListById(int id)
    {
        var priceList = await _context.PriceLists
            .Include(p => p.PriceItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (priceList == null)
        {
            return NotFound(new
            {
                message = "Price list not found."
            });
        }

        return Ok(priceList);
    }

    // POST: api/pricelists
    [HttpPost]
    public async Task<ActionResult<PriceList>> CreatePriceList(
        PriceList priceList)
    {
        if (string.IsNullOrWhiteSpace(priceList.Name))
        {
            return BadRequest(new
            {
                message = "Price list name is required."
            });
        }

        if (priceList.EffectiveDate == default)
        {
            return BadRequest(new
            {
                message = "Effective date is required."
            });
        }

        priceList.Id = 0;
        priceList.Name = priceList.Name.Trim();

        // Không tạo PriceItem thông qua API tạo PriceList
        priceList.PriceItems = new List<PriceItem>();

        _context.PriceLists.Add(priceList);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPriceListById),
            new { id = priceList.Id },
            priceList
        );
    }
}