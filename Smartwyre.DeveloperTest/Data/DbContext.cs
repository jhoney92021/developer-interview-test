using Microsoft.EntityFrameworkCore;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class SmartwyreDbContext : DbContext
{
    public SmartwyreDbContext(DbContextOptions<SmartwyreDbContext> options) : base(options) { }

    public DbSet<Rebate> Rebates { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<RebateCalculationLog> CalculationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map Identifier as the Primary Key for Rebate
        modelBuilder.Entity<Rebate>()
            .HasKey(r => r.Identifier);

        // Map Identifier as the Primary Key for Product
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Identifier);
        
        modelBuilder.Entity<RebateCalculationLog>()
            .ToTable("RebateCalculationLogs");
    }
}
