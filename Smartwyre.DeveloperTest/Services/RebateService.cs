using Microsoft.Extensions.Logging;
using Smartwyre.DeveloperTest.Data.Abstracts;
using Smartwyre.DeveloperTest.Extensions;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly ILogger<RebateService> _logger;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        ILogger<RebateService> logger)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _logger = logger;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        CalculateRebateResult result = new();

        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (rebate == null)
        {
            _logger.LogWarning("Rebate calculation failed: Rebate record '{RebateId}' not found.", request.RebateIdentifier);
            result.Success = false;
            return result;
        }

        if (product == null)
        {
            _logger.LogWarning("Rebate calculation failed: Product record '{ProductId}' not found.", request.ProductIdentifier);
            result.Success = false;
            return result;
        }

        SupportedIncentiveType targetFlag = rebate.Incentive.ToSupportedFlag();
        if (!product.SupportedIncentives.HasFlag(targetFlag))
        {
            _logger.LogWarning("Rebate calculation failed: Product '{ProductId}' does not support incentive type '{Incentive}'. Required flag: {TargetFlag}",
                product.Identifier, rebate.Incentive, targetFlag);
            result.Success = false;
            return result;
        }

        if (rebate.Incentive.HasValidInputs(rebate, product, request.Volume))
        {
            decimal rebateAmount = rebate.Incentive.CalculateAmount(rebate, product, request.Volume);
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
            result.Success = true;
        }
        else
        {
            _logger.LogCritical("Rebate calculation structural violation: Zero or invalid metrics provided for rule strategy '{Incentive}'. Rebate Amount: {RebateAmount}, Product Price: {Price}, Request Volume: {Volume}",
                rebate.Incentive, rebate.Amount, product.Price, request.Volume);
            result.Success = false;
        }

        return result;
    }
}
