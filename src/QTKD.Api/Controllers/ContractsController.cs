using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Api.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly QtkdDbContext _context;

    public ContractsController(QtkdDbContext context)
    {
        _context = context;
    }

    // GET: api/contracts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContracts()
    {
        var contracts = await _context.Contracts
            .Include(c => c.Customer)
            .ToListAsync();

        return Ok(contracts);
    }

    // GET: api/contracts/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Contract>> GetContractById(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
        {
            return NotFound(new
            {
                message = $"Contract with ID {id} not found."
            });
        }

        return Ok(contract);
    }

    // POST: api/contracts
    [HttpPost]
    public async Task<ActionResult<Contract>> CreateContract(
        Contract contract)
    {
        if (string.IsNullOrWhiteSpace(contract.ContractNo))
        {
            return BadRequest(new
            {
                message = "Contract number is required."
            });
        }

        if (contract.EndDate <= contract.StartDate)
        {
            return BadRequest(new
            {
                message = "End date must be after start date."
            });
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == contract.CustomerId);

        if (customer == null || customer.Status != "Active")
        {
            return BadRequest(new
            {
                message = "Customer does not exist or is inactive."
            });
        }

        var exists = await _context.Contracts
            .AnyAsync(c => c.ContractNo == contract.ContractNo);

        if (exists)
        {
            return Conflict(new
            {
                message = "Contract number already exists."
            });
        }

        contract.Id = 0;
        contract.ContractNo = contract.ContractNo.Trim();
        contract.Status = "Draft";

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetContractById),
            new { id = contract.Id },
            contract
        );
    }

    // PUT: api/contracts/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateContract(
        int id, Contract updatedContract)
    {
        var contract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
        {
            return NotFound(new
            {
                message = "Contract not found."
            });
        }

        // Không cho phép thay đổi trạng thái qua API cập nhật
        if (updatedContract.Status != contract.Status)
        {
            return BadRequest(new
            {
                message = "Status cannot be changed through this endpoint."
            });
        }
        
        // CTR-01: Chỉ sửa Draft hoặc Revision Requested
        if (contract.Status != "Draft" &&
            contract.Status != "Revision Requested")
        {
            return BadRequest(new
            {
                message = "Only Draft or Revision Requested contracts can be updated."
            });
        }

        if (string.IsNullOrWhiteSpace(updatedContract.ContractNo))
        {
            return BadRequest(new
            {
                message = "Contract number is required."
            });
        }

        if (updatedContract.EndDate <= updatedContract.StartDate)
        {
            return BadRequest(new
            {
                message = "End date must be after start date."
            });
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == updatedContract.CustomerId);

        if (customer == null || customer.Status != "Active")
        {
            return BadRequest(new
            {
                message = "Customer does not exist or is inactive."
            });
        }

        var duplicate = await _context.Contracts.AnyAsync(
            c => c.ContractNo == updatedContract.ContractNo
                 && c.Id != id);

        if (duplicate)
        {
            return Conflict(new
            {
                message = "Contract number already exists."
            });
        }

        contract.ContractNo = updatedContract.ContractNo.Trim();
        contract.CustomerId = updatedContract.CustomerId;
        contract.StartDate = updatedContract.StartDate;
        contract.EndDate = updatedContract.EndDate;
        contract.Description = updatedContract.Description?.Trim() ?? "";
        contract.AttachmentName = updatedContract.AttachmentName?.Trim() ?? "";

        // Không cho phép client tự ý thay đổi Status
        await _context.SaveChangesAsync();

        return Ok(contract);
    }

        // POST: api/contracts/{id}/submit
    [HttpPost("{id:int}/submit")]
    public async Task<IActionResult> SubmitContract(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        // Kiểm tra Contract có tồn tại
        if (contract == null)
        {
            return NotFound(new
            {
                message = "Contract not found."
            });
        }
        
        // CTR-03: Không cho phép gửi duyệt nếu không ở trạng thái Draft
        if (contract.Status != "Draft")
        {
            return BadRequest(new
            {
                message = "Only Draft contracts can be submitted."
            });
        }

        // Kiểm tra Customer
        if (contract.Customer == null ||
            contract.Customer.Status != "Active")
        {
            return BadRequest(new
            {
                message = "Contract must have a valid active customer."
            });
        }

        // CTR-02: Kiểm tra ngày hiệu lực
        if (contract.StartDate == default ||
            contract.EndDate <= contract.StartDate)
        {
            return BadRequest(new
            {
                message = "Contract effective dates are invalid."
            });
        }

        // CTR-02: Kiểm tra nội dung và tài liệu đính kèm
        if (string.IsNullOrWhiteSpace(contract.Description))
        {
            return BadRequest(new
            {
                message = "Contract description is required."
            });
        }

        if (string.IsNullOrWhiteSpace(contract.AttachmentName))
        {
            return BadRequest(new
            {
                message = "Contract attachment is required."
            });
        }

        // CTR-03: Submit thành công chỉ chuyển sang PendingApproval
        contract.Status = "PendingApproval";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Contract submitted successfully.",
            contractId = contract.Id,
            contractNo = contract.ContractNo,
            status = contract.Status
        });
    }
}