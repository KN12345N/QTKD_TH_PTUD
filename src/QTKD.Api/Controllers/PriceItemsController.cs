using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Api.Controllers;

[ApiController]
[Route("api/priceitems")]
public class PriceItemsController : ControllerBase
{
    private readonly QtkdDbContext _context;

    public PriceItemsController(QtkdDbContext context)
    {
        _context = context;
    }

    // GET: api/priceitems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriceItem>>> GetPriceItems()
    {
        var priceItems = await _context.PriceItems
            .ToListAsync();

        return Ok(priceItems);
    }

    // GET: api/priceitems/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PriceItem>> GetPriceItemById(int id)
    {
        var priceItem = await _context.PriceItems
            .FirstOrDefaultAsync(p => p.Id == id);

        if (priceItem == null)
        {
            return NotFound(new
            {
                message = "Price item not found."
            });
        }

        return Ok(priceItem);
    }

    // POST: api/priceitems
    [HttpPost]
    public async Task<ActionResult<PriceItem>> CreatePriceItem(
        PriceItem priceItem)
    {
        if (string.IsNullOrWhiteSpace(priceItem.ServiceCode))
        {
            return BadRequest(new
            {
                message = "Service code is required."
            });
        }

        if (priceItem.UnitPrice <= 0)
        {
            return BadRequest(new
            {
                message = "Unit price must be greater than zero."
            });
        }

        var priceList = await _context.PriceLists
            .FirstOrDefaultAsync(p => p.Id == priceItem.PriceListId);

        if (priceList == null)
        {
            return BadRequest(new
            {
                message = "Price list does not exist."
            });
        }

        priceItem.Id = 0;
        priceItem.ServiceCode = priceItem.ServiceCode.Trim();
        priceItem.PriceList = null;

        _context.PriceItems.Add(priceItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPriceItemById),
            new { id = priceItem.Id },
            priceItem
        );
    }
}