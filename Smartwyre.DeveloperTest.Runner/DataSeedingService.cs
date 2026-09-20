using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using System.Linq;

namespace Smartwyre.DeveloperTest.Runner;

public static class DataSeedingService
{
    public static void SeedData(SmartwyreDbContext context, bool forceClear = false)
    {
        if (forceClear)
        {
            // Wipe existing records cleanly if requested by the user flag
            context.Rebates.RemoveRange(context.Rebates);
            context.Products.RemoveRange(context.Products);
            context.SaveChanges();
        }

        // 1. Seed or update missing Rebates
        SeedRebateIfMissing(context, "REBATE_FIXED_CASH", IncentiveType.FixedCashAmount, 100m, 0m);
        SeedRebateIfMissing(context, "REBATE_FIXED_RATE", IncentiveType.FixedRateRebate, 0m, 0.10m);
        SeedRebateIfMissing(context, "REBATE_UOM", IncentiveType.AmountPerUom, 5m, 0m);

        // 2. Seed or update missing Products
        SeedProductIfMissing(context, "PROD_FIXED_CASH", SupportedIncentiveType.FixedCashAmount, 50m);
        SeedProductIfMissing(context, "PROD_FIXED_RATE", SupportedIncentiveType.FixedRateRebate, 200m);
        SeedProductIfMissing(context, "PROD_UOM", SupportedIncentiveType.AmountPerUom, 15m);

        context.SaveChanges();
    }

    private static void SeedRebateIfMissing(SmartwyreDbContext context, string id, IncentiveType type, decimal amount, decimal percentage)
    {
        Rebate existingRebate = context.Rebates.FirstOrDefault(r => r.Identifier == id);
        if (existingRebate == null)
        {
            context.Rebates.Add(new Rebate { Identifier = id, Incentive = type, Amount = amount, Percentage = percentage });
        }
        else
        {
            // Update values seamlessly if the dataset definition changed
            existingRebate.Incentive = type;
            existingRebate.Amount = amount;
            existingRebate.Percentage = percentage;
        }
    }

    private static void SeedProductIfMissing(SmartwyreDbContext context, string id, SupportedIncentiveType flags, decimal price)
    {
        Product existingProduct = context.Products.FirstOrDefault(p => p.Identifier == id);
        if (existingProduct == null)
        {
            context.Products.Add(new Product { Identifier = id, SupportedIncentives = flags, Price = price });
        }
        else
        {
            // Update values seamlessly if the dataset definition changed
            existingProduct.SupportedIncentives = flags;
            existingProduct.Price = price;
        }
    }
}
