using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QTKD.Api.Controllers;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Tests;

public class CustomerTests
{
    private QtkdDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<QtkdDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new QtkdDbContext(options);
    }

    // TC01: Tạo Customer hợp lệ
    [Fact]
    public async Task CreateCustomer_ValidData_ReturnsCreated()
    {
        using var context = CreateContext();
        var controller = new CustomersController(context);

        var customer = new Customer
        {
            Name = "Công ty 123",
            Status = "Active"
        };

        var result = await controller.CreateCustomer(customer);

        // Kiểm tra HTTP 201 Created
        var createdResult = Assert.IsType<CreatedAtActionResult>(
            result.Result);

        // Kiểm tra dữ liệu trả về
        var createdCustomer = Assert.IsType<Customer>(
            createdResult.Value);

        Assert.Equal("Công ty 123", createdCustomer.Name);
        Assert.Equal("Active", createdCustomer.Status);

        // Kiểm tra dữ liệu đã được lưu
        var savedCustomer = await context.Customers
            .FindAsync(createdCustomer.Id);

        Assert.NotNull(savedCustomer);
    }

    // TC02a: Tạo Customer thiếu tên
    [Fact]
    public async Task CreateCustomer_MissingName_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new CustomersController(context);

        var customer = new Customer
        {
            Name = "",
            Status = "Active"
        };

        var result = await controller.CreateCustomer(customer);

        Assert.IsType<BadRequestObjectResult>(result.Result);

        // Không được lưu dữ liệu không hợp lệ
        Assert.Equal(0, await context.Customers.CountAsync());
    }

    // TC02b: Tạo Customer thiếu trạng thái
    [Fact]
    public async Task CreateCustomer_MissingStatus_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new CustomersController(context);

        var customer = new Customer
        {
            Name = "Công ty 123",
            Status = ""
        };

        var result = await controller.CreateCustomer(customer);

        Assert.IsType<BadRequestObjectResult>(result.Result);

        // Không được lưu dữ liệu không hợp lệ
        Assert.Equal(0, await context.Customers.CountAsync());
    }
}