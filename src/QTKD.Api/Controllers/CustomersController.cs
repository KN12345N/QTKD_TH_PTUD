using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly QtkdDbContext _context;

    public CustomerController(QtkdDbContext context)
    {
        _context = context;
    }

    // GET: api/Customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }
}