using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly QtkdDbContext _context;

    public CustomersController(QtkdDbContext context)
    {
        _context = context;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await _context.Customers.ToListAsync();
        return Ok(customers);
    }

    // GET: api/customers/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Customer>> GetCustomerById(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            return NotFound(new
            {
                message = $"Customer with ID {id} not found."
            });
        }

        return Ok(customer);
    }

    // POST: api/customers
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            return BadRequest(new
            {
                message = "Customer name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(customer.Status))
        {
            return BadRequest(new
            {
                message = "Customer status is required."
            });
        }

        customer.Id = 0;
        customer.Name = customer.Name.Trim();
        customer.Status = customer.Status.Trim();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCustomerById),
            new { id = customer.Id },
            customer
        );
    }

    // PUT: api/customers/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound(new
            {
                message = $"Customer with ID {id} not found."
            });
        }

        if (string.IsNullOrWhiteSpace(updatedCustomer.Name))
        {
            return BadRequest(new
            {
                message = "Customer name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(updatedCustomer.Status))
        {
            return BadRequest(new
            {
                message = "Customer status is required."
            });
        }

        customer.Name = updatedCustomer.Name.Trim();
        customer.Status = updatedCustomer.Status.Trim();

        await _context.SaveChangesAsync();

        return Ok(customer);
    }
}