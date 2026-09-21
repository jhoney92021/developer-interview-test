using Smartwyre.DeveloperTest.Data.Abstracts;
using Smartwyre.DeveloperTest.Types;
using System;
using System.Linq;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore : IProductDataStore, IDisposable
{
    private readonly SmartwyreDbContext _context;
    private bool _disposed = false;

    public ProductDataStore(SmartwyreDbContext context)
    {
        _context = context;
    }

    public Product GetProduct(string productIdentifier)
    {
        return _context.Products.FirstOrDefault(p => p.Identifier == productIdentifier);
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
