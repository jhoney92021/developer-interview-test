using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Data.Abstracts;

public interface IRebateDataStore
{
    public Rebate GetRebate(string rebateIdentifier);
    public void StoreCalculationResult(Rebate account, decimal rebateAmount);
}
