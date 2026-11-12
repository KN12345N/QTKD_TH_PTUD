using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using QTKD.Api.Data;
using QTKD.Api.Entities;

namespace QTKD.Tests;

public class CustomerIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CustomerIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Test case: Tạo Customer với dữ liệu hợp lệ
    [Fact]
    public async Task CreateCustomer_ValidData_ReturnsCreatedAndSaved()
    {
        var request = new
        {
            name = "Khách hàng Integration Test",
            status = "Active"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/customers", request);

        // Kiểm tra HTTP status
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Kiểm tra nội dung phản hồi
        var customer = await response.Content
            .ReadFromJsonAsync<Customer>();

        Assert.NotNull(customer);
        Assert.Equal("Khách hàng Integration Test", customer.Name);
        Assert.Equal("Active", customer.Status);
        Assert.True(customer.Id > 0);

        // Kiểm tra dữ liệu đã lưu trong database kiểm thử
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<QtkdDbContext>();

        var saved = await db.Customers
            .FirstOrDefaultAsync(c => c.Id == customer.Id);

        Assert.NotNull(saved);
        Assert.Equal(customer.Name, saved.Name);
        Assert.Equal(customer.Status, saved.Status);
    }

    // Test case: Tạo Customer với tên trống
    [Fact]
    public async Task CreateCustomer_MissingName_ReturnsBadRequest()
    {
        var request = new
        {
            name = "",
            status = "Active"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/customers", request);

        // Kiểm tra API từ chối yêu cầu
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        // Kiểm tra nội dung phản hồi
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Customer name is required.", content);

        // Kiểm tra dữ liệu không được lưu
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<QtkdDbContext>();

        var exists = await db.Customers.AnyAsync(
            c => c.Name == "");

        Assert.False(exists);
    }
}