using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Data.Abstracts;

public interface IProductDataStore
{
    public Product GetProduct(string productIdentifier);
}
