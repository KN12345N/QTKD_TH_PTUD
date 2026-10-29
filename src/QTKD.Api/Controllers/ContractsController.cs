using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly QtkdDbContext _context;

    public ContractsController(QtkdDbContext context)
    {
        _context = context;
    }

    // GET: api/contracts/{id}
    [HttpGet("id")]
    public async Task<ActionResult<Contract>> GetContractById(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
        {
            return NotFound(new { message = $"Contract with ID {id} not found." });
        }

        return Ok(contract);
    }
}