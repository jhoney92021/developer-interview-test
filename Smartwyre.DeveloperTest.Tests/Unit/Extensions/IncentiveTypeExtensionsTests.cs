using Smartwyre.DeveloperTest.Extensions;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Unit.Extensions;

public class IncentiveTypeExtensionsTests
{
    [Theory]
    // FixedCashAmount: Returns the rebate amount directly (ignores price/volume math)
    [InlineData(IncentiveType.FixedCashAmount, 500, 50, 10, 500)]
    // FixedRateRebate: Price (200) * Percentage (0.10) * Volume (5) = 100
    [InlineData(IncentiveType.FixedRateRebate, 0, 200, 5, 100)]
    // AmountPerUom: Rebate Amount (15) * Volume (4) = 60
    [InlineData(IncentiveType.AmountPerUom, 15, 100, 4, 60)]
    public void CalculateAmount_ShouldReturnExpectedValue_ForAllIncentiveTypes(
        IncentiveType incentive,
        decimal rebateAmount,
        decimal productPrice,
        decimal volume,
        decimal expectedAmount)
    {
        Rebate rebate = new() { Amount = rebateAmount, Percentage = 0.10m }; // Setup percentage default for FixedRate pass
        Product product = new() { Price = productPrice };

        decimal actualAmount = incentive.CalculateAmount(rebate, product, volume);

        Assert.Equal(expectedAmount, actualAmount);
    }

    [Theory]
    // FixedCashAmount boundaries
    [InlineData(IncentiveType.FixedCashAmount, 100, 0, 1, true)]
    [InlineData(IncentiveType.FixedCashAmount, 0, 0, 1, false)]
    // FixedRateRebate boundaries
    [InlineData(IncentiveType.FixedRateRebate, 0, 50, 10, true)]   // Valid (uses internal default percentage)
    [InlineData(IncentiveType.FixedRateRebate, 0, 50, 0, false)]   // Zero Volume
    [InlineData(IncentiveType.FixedRateRebate, 0, 0, 10, false)]   // Zero Price
    // AmountPerUom boundaries
    [InlineData(IncentiveType.AmountPerUom, 25, 0, 5, true)]       // Valid
    [InlineData(IncentiveType.AmountPerUom, 0, 0, 5, false)]       // Zero Amount
    [InlineData(IncentiveType.AmountPerUom, 25, 0, 0, false)]      // Zero Volume
    public void HasValidInputs_ShouldValidateCorrectly_ForAllIncentiveTypes(
        IncentiveType incentive,
        decimal rebateAmount,
        decimal productPrice,
        decimal volume,
        bool expectedResult)
    {
        // Set a default working percentage of 0.10m for FixedRate validation paths unless testing an explicit fallback
        decimal targetPercentage = (incentive == IncentiveType.FixedRateRebate && rebateAmount == 0) ? 0.10m : 0.0m;

        Rebate rebate = new() { Amount = rebateAmount, Percentage = targetPercentage };
        Product product = new() { Price = productPrice };

        bool actualResult = incentive.HasValidInputs(rebate, product, volume);

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    // Direct single matching loops
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, true)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, true)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount, true)]
    // Mixed product setups (Product supports multiple incentive paths)
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom, true)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate, true)]
    // Direct failures / mismatches
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.AmountPerUom, false)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedCashAmount, false)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate, false)]
    public void ToSupportedFlag_ShouldAccuratelyMap_AllPermutationsAgainstProductFlags(
    IncentiveType incentive,
    SupportedIncentiveType productFlags,
    bool expectedMatch)
    {
        // Act - Convert the enum using our safe switch expression
        SupportedIncentiveType targetFlag = incentive.ToSupportedFlag();
        bool actualMatch = productFlags.HasFlag(targetFlag);

        // Assert
        Assert.Equal(expectedMatch, actualMatch);
    }
}