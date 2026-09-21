using Microsoft.EntityFrameworkCore;
using Smartwyre.DeveloperTest.Data;

namespace Smartwyre.DeveloperTest.Runner;

public static class DataStoreFactory
{
    private const string ConnectionString = "Data Source=smartwyre.db";

    private static DbContextOptions<SmartwyreDbContext> GetOptions()
    {
        DbContextOptionsBuilder<SmartwyreDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite(ConnectionString);
        return optionsBuilder.Options;
    }

    public static RebateDataStore CreateRebateStore(bool forceSeed = false)
    {
        SmartwyreDbContext context = new(GetOptions());
        context.Database.EnsureCreated();
        DataSeedingService.SeedData(context, forceSeed);
        return new RebateDataStore(context);
    }

    public static ProductDataStore CreateProductStore(bool forceSeed = false)
    {
        SmartwyreDbContext context = new(GetOptions());
        context.Database.EnsureCreated();
        DataSeedingService.SeedData(context, forceSeed);
        return new ProductDataStore(context);
    }
}
