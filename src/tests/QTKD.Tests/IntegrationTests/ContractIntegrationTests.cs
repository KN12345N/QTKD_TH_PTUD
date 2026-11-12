using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using QTKD.Api.Entities;

namespace QTKD.Tests;

public class ContractIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContractIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // Test case: Tạo hợp đồng, submit và đọc trạng thái
    [Fact]
    public async Task ContractWorkflow_CreateSubmitAndReadStatus()
    {
        // 1. Tạo Customer
        var customerRequest = new
        {
            name = "Customer Contract Test",
            status = "Active"
        };

        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers", customerRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            customerResponse.StatusCode);

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<Customer>();

        Assert.NotNull(customer);

        // 2. Tạo Contract
        var contractRequest = new
        {
            contractNo = "HD-INTEGRATION-001",
            customerId = customer.Id,
            startDate = new DateTime(2026, 1, 1),
            endDate = new DateTime(2026, 12, 31),
            description = "Hợp đồng kiểm thử tích hợp",
            attachmentName = "integration.pdf"
        };

        var contractResponse = await _client.PostAsJsonAsync(
            "/api/contracts", contractRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            contractResponse.StatusCode);

        var contract = await contractResponse.Content
            .ReadFromJsonAsync<Contract>();

        Assert.NotNull(contract);
        Assert.True(contract.Id > 0);
        Assert.Equal("Draft", contract.Status);

        // 3. Submit Contract
        var submitResponse = await _client.PostAsync(
            $"/api/contracts/{contract.Id}/submit", null);

        Assert.Equal(
            HttpStatusCode.OK,
            submitResponse.StatusCode);

        // 4. Đọc lại Contract qua API
        var getResponse = await _client.GetAsync(
            $"/api/contracts/{contract.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var savedContract = await getResponse.Content
            .ReadFromJsonAsync<Contract>();

        Assert.NotNull(savedContract);
        Assert.Equal("PendingApproval", savedContract.Status);
        Assert.Equal(
            "HD-INTEGRATION-001",
            savedContract.ContractNo);
    }

    // Test case: Submit hợp đồng hai lần, lần thứ hai trả về BadRequest
    [Fact]
    public async Task SubmitContract_Twice_ReturnsBadRequest()
    {
        // Tạo Customer
        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            new
            {
                name = "Customer Submit Twice",
                status = "Active"
            });

        Assert.Equal(HttpStatusCode.Created, customerResponse.StatusCode);

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<Customer>();

        Assert.NotNull(customer);

        // Tạo Contract
        var contractResponse = await _client.PostAsJsonAsync(
            "/api/contracts",
            new
            {
                contractNo = "HD-SUBMIT-TWICE",
                customerId = customer.Id,
                startDate = new DateTime(2026, 1, 1),
                endDate = new DateTime(2026, 12, 31),
                description = "Kiểm thử Submit hai lần",
                attachmentName = "test.pdf"
            });

        Assert.Equal(HttpStatusCode.Created, contractResponse.StatusCode);

        var contract = await contractResponse.Content
            .ReadFromJsonAsync<Contract>();

        Assert.NotNull(contract);

        // Submit lần thứ nhất
        var firstSubmit = await _client.PostAsync(
            $"/api/contracts/{contract.Id}/submit", null);

        Assert.Equal(HttpStatusCode.OK, firstSubmit.StatusCode);

        // Submit lần thứ hai
        var secondSubmit = await _client.PostAsync(
            $"/api/contracts/{contract.Id}/submit", null);

        Assert.Equal(HttpStatusCode.BadRequest, secondSubmit.StatusCode);

        // Đọc lại trạng thái
        var getResponse = await _client.GetAsync(
            $"/api/contracts/{contract.Id}");

        var saved = await getResponse.Content
            .ReadFromJsonAsync<Contract>();

        Assert.NotNull(saved);
        Assert.Equal("PendingApproval", saved.Status);
    }
}