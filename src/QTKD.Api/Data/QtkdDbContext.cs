using Microsoft.EntityFrameworkCore;
using QTKD.Api.Entities;

namespace QTKD.Api.Data;

public class QtkdDbContext : DbContext
{
    public QtkdDbContext(DbContextOptions<QtkdDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceItem> PriceItems => Set<PriceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình quan hệ Contract - Customer (1-N)
        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Customer)
            .WithMany(cust => cust.Contracts)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // 2. Cấu hình quan hệ PriceList - PriceItem (1-N)
        modelBuilder.Entity<PriceItem>()
            .HasOne(pi => pi.PriceList)
            .WithMany(pl => pl.PriceItems)
            .HasForeignKey(pi => pi.PriceListId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // 3. Nạp dữ liệu mẫu
        // 3.1. Ít nhất 2 Customer
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Công ty Minh Phát", Status = "Active" },
            new Customer { Id = 2, Name = "Công ty ABC", Status = "Active" }
        );

        // 3.2. 1 Contract ở trạng thái Draft
        modelBuilder.Entity<Contract>().HasData(
            new Contract { 
                Id = 1,
                ContractNo = "HD-2026-018", 
                CustomerId = 1,
                StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc), 
                Status = "Draft" 
            }
        );

        // 3.3. 1 PriceList
        modelBuilder.Entity<PriceList>().HasData(
            new PriceList { 
                Id = 1, 
                Name = "Price List 2026", 
                EffectiveDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // 3.4. 2 PriceItem thuộc PriceList trên
        modelBuilder.Entity<PriceItem>().HasData(
            new PriceItem { 
                Id = 1, 
                PriceListId = 1, 
                ServiceCode = "SRV_IT_01", 
                UnitPrice = 15000000m
            },
            new PriceItem { 
                Id = 2, 
                PriceListId = 1, 
                ServiceCode = "SRV_IT_02", 
                UnitPrice = 25000000m 
            }
        );
    }
}