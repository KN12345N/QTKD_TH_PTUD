using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QTKD.Api.Data;

namespace QTKD.Tests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                IDbContextOptionsConfiguration<QtkdDbContext>>();

            services.RemoveAll<
                DbContextOptions<QtkdDbContext>>();

            services.AddDbContext<QtkdDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}