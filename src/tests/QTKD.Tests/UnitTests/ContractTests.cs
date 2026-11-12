using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QTKD.Api.Controllers;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Tests;

public class ContractTests
{
    private QtkdDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<QtkdDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new QtkdDbContext(options);

        // Tạo dữ liệu mẫu
        context.Customers.Add(new Customer
        {
            Id = 1,
            Name = "Công ty Minh Phát",
            Status = "Active"
        });

        context.Contracts.Add(new Contract
        {
            Id = 1,
            ContractNo = "HD-2026-018",
            CustomerId = 1,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Status = "Draft",
            Description = "Hợp đồng dịch vụ",
            AttachmentName = "hopdong.pdf"
        });

        context.SaveChanges();

        return context;
    }

    // Sửa trực tiếp trạng thái Contract
    [Fact]
    public async Task UpdateContract_TrySetApproved_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new ContractsController(context);

        var updatedContract = new Contract
        {
            ContractNo = "HD-2026-018",
            CustomerId = 1,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Description = "Hợp đồng dịch vụ",
            AttachmentName = "hopdong.pdf",
            Status = "Approved"
        };

        var result = await controller.UpdateContract(1, updatedContract);

        Assert.IsType<BadRequestObjectResult>(result);

        var saved = await context.Contracts.FindAsync(1);
        Assert.NotNull(saved);
        Assert.Equal("Draft", saved.Status);
    }

    // TC03: Sửa Contract ở trạng thái Draft
    [Fact]
    public async Task UpdateContract_Draft_ReturnsOk()
    {
        using var context = CreateContext();
        var controller = new ContractsController(context);

        var updatedContract = new Contract
        {
            ContractNo = "HD-2026-019",
            CustomerId = 1,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Description = "Nội dung đã cập nhật",
            AttachmentName = "updated.pdf"
        };

        var result = await controller.UpdateContract(1, updatedContract);

        Assert.IsType<OkObjectResult>(result);

        var saved = await context.Contracts.FindAsync(1);

        Assert.NotNull(saved);
        Assert.Equal("HD-2026-019", saved.ContractNo);
        Assert.Equal("Nội dung đã cập nhật", saved.Description);
    }

    // TC04: Không cho sửa Contract ở PendingApproval
    [Fact]
    public async Task UpdateContract_PendingApproval_ReturnsBadRequest()
    {
        using var context = CreateContext();

        var contract = await context.Contracts.FindAsync(1);
        contract!.Status = "PendingApproval";
        await context.SaveChangesAsync();

        var controller = new ContractsController(context);

        var updatedContract = new Contract
        {
            ContractNo = "HD-2026-020",
            CustomerId = 1,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31)
        };

        var result = await controller.UpdateContract(1, updatedContract);

        Assert.IsType<BadRequestObjectResult>(result);

        var saved = await context.Contracts.FindAsync(1);

        Assert.Equal("HD-2026-018", saved!.ContractNo);
        Assert.Equal("PendingApproval", saved.Status);
    }

    // TC05: Submit thiếu nội dung hợp đồng
    [Fact]
    public async Task SubmitContract_MissingDescription_ReturnsBadRequest()
    {
        using var context = CreateContext();

        var contract = await context.Contracts.FindAsync(1);
        contract!.Description = "";
        await context.SaveChangesAsync();

        var controller = new ContractsController(context);

        var result = await controller.SubmitContract(1);

        Assert.IsType<BadRequestObjectResult>(result);

        var saved = await context.Contracts.FindAsync(1);

        Assert.Equal("Draft", saved!.Status);
    }

    // TC06: Submit hợp đồng hợp lệ
    [Fact]
    public async Task SubmitContract_ValidData_ReturnsPendingApproval()
    {
        using var context = CreateContext();
        var controller = new ContractsController(context);

        var result = await controller.SubmitContract(1);

        Assert.IsType<OkObjectResult>(result);

        var saved = await context.Contracts.FindAsync(1);

        Assert.Equal("PendingApproval", saved!.Status);
    }

    // TC08: Không cho Submit cùng một Contract hai lần
    [Fact]
    public async Task SubmitContract_Twice_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new ContractsController(context);

        var firstResult = await controller.SubmitContract(1);
        var secondResult = await controller.SubmitContract(1);

        Assert.IsType<OkObjectResult>(firstResult);
        Assert.IsType<BadRequestObjectResult>(secondResult);

        var saved = await context.Contracts.FindAsync(1);

        Assert.Equal("PendingApproval", saved!.Status);
    }
}