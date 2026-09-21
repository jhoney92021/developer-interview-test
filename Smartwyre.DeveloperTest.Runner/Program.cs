using System;
using Microsoft.Extensions.Logging.Abstractions;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: dotnet run -- [RebateIdentifier] [ProductIdentifier] [Volume]");
            return;
        }

        // Map command-line inputs using explicit array indexes
        string rebateCode = args[0];
        string productIdentifier = args[1];

        if (!decimal.TryParse(args[2], out decimal volume))
        {
            Console.WriteLine("Error: Volume must be a valid numeric decimal.");
            return;
        }

        CalculateRebateRequest request = new()
        {
            RebateIdentifier = rebateCode,
            ProductIdentifier = productIdentifier,
            Volume = volume
        };

        // DataStoreFactory handles Database creation, table mapping, and data seeding on boot
        using RebateDataStore rebateDataStore = DataStoreFactory.CreateRebateStore();
        using ProductDataStore productDataStore = DataStoreFactory.CreateProductStore();

        // Instantiate the service using Dependency Injection via the constructor
        RebateService rebateService = new(rebateDataStore, productDataStore, NullLogger<RebateService>.Instance);

        CalculateRebateResult result = rebateService.Calculate(request);

        Console.WriteLine($"Result Success Status: {result.Success}");
    }
}
