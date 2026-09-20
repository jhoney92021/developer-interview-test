using Smartwyre.DeveloperTest.Extensions;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Unit.Extensions;

public class IncentiveEnumStressTests
{
    [Theory]
    // 1. Single Direct Matches
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, true)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, true)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount, true)]

    // 2. Dual-Flag Combinations (Product supports exactly two incentives)
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom, true)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.FixedCashAmount, true)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedRateRebate, true)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedCashAmount, true)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate, true)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.AmountPerUom, true)]

    // 3. Multi-Flag Combinations (Product supports ALL incentives)
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedCashAmount, true)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedCashAmount, true)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedCashAmount, true)]

    // 4. Exclusions / Direct Failures (Product completely lacks the matching bit flag)
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.AmountPerUom, false)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedCashAmount, false)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedCashAmount, false)]

    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedRateRebate, false)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedCashAmount, false)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.FixedCashAmount, false)]

    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate, false)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.AmountPerUom, false)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom, false)]

    // 5. Empty Flag Edge Case (Product explicitly supports zero incentives)
    [InlineData(IncentiveType.FixedRateRebate, (SupportedIncentiveType)0, false)]
    [InlineData(IncentiveType.AmountPerUom, (SupportedIncentiveType)0, false)]
    [InlineData(IncentiveType.FixedCashAmount, (SupportedIncentiveType)0, false)]
    public void Verify_EnumCompatibility_AgainstAllComplexBitmaskCombinations(
        IncentiveType incentive,
        SupportedIncentiveType productFlags,
        bool expectedResult)
    {
        // Act - Run the chosen resolution mapping strategy
        SupportedIncentiveType targetFlag = incentive.ToSupportedFlag();
        bool actualResult = productFlags.HasFlag(targetFlag);

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }
}
