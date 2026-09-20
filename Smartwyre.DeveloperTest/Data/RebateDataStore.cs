using Smartwyre.DeveloperTest.Data.Abstracts;
using Smartwyre.DeveloperTest.Types;
using System;
using System.Linq;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore : IRebateDataStore, IDisposable
{
    private readonly SmartwyreDbContext _context;
    private bool _disposed;

    public RebateDataStore(SmartwyreDbContext context)
    {
        _context = context;
    }

    public Rebate GetRebate(string rebateIdentifier)
    {
        return _context.Rebates.FirstOrDefault(r => r.Identifier == rebateIdentifier);
    }

    public void StoreCalculationResult(Rebate rebate, decimal amount)
    {
        _context.CalculationLogs.Add(new RebateCalculationLog
        {
            RebateIdentifier = rebate.Identifier,
            Amount = amount
        });
        _context.SaveChanges();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Clean up the DB context reference held by this store
                _context?.Dispose();
            }
            _disposed = true;
        }
    }
}