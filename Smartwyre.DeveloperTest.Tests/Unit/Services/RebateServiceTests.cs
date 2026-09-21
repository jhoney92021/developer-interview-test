using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Smartwyre.DeveloperTest.Data.Abstracts;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Unit.Services;

public class RebateServiceTests
{
    private readonly Mock<IRebateDataStore> _mockRebateStore;
    private readonly Mock<IProductDataStore> _mockProductStore;
    private readonly RebateService _service;

    public RebateServiceTests()
    {
        _mockRebateStore = new Mock<IRebateDataStore>();
        _mockProductStore = new Mock<IProductDataStore>();
        _service = new RebateService(_mockRebateStore.Object, _mockProductStore.Object, NullLogger<RebateService>.Instance);
    }

    [Theory]
    // 1. Missing Entity Failures
    [InlineData("MISSING_REBATE", "PROD_OK", IncentiveType.FixedCashAmount, 100, 0, SupportedIncentiveType.FixedCashAmount, 50, 1, false, false)]
    [InlineData("REBATE_OK", "MISSING_PROD", IncentiveType.FixedCashAmount, 100, 0, SupportedIncentiveType.FixedCashAmount, 50, 1, true, false)]

    // 2. Mismatched Incentive Flag Failures
    [InlineData("R1", "P1", IncentiveType.FixedCashAmount, 100, 0, SupportedIncentiveType.FixedRateRebate, 50, 1, true, true)]
    [InlineData("R2", "P2", IncentiveType.FixedRateRebate, 0, 0.10, SupportedIncentiveType.AmountPerUom, 200, 10, true, true)]

    // 3. Zero Metric Failures (FixedCashAmount amount = 0)
    [InlineData("R3", "P3", IncentiveType.FixedCashAmount, 0, 0, SupportedIncentiveType.FixedCashAmount, 50, 1, true, true)]

    // 4. Zero Metric Failures (FixedRateRebate variations)
    [InlineData("R4", "P4", IncentiveType.FixedRateRebate, 0, 0, SupportedIncentiveType.FixedRateRebate, 100, 20, true, true)] // Zero percentage
    [InlineData("R5", "P5", IncentiveType.FixedRateRebate, 0, 0.05, SupportedIncentiveType.FixedRateRebate, 0, 20, true, true)] // Zero price
    [InlineData("R6", "P6", IncentiveType.FixedRateRebate, 0, 0.05, SupportedIncentiveType.FixedRateRebate, 100, 0, true, true)]  // Zero volume

    // 5. Zero Metric Failures (AmountPerUom variations)
    [InlineData("R7", "P7", IncentiveType.AmountPerUom, 0, 0, SupportedIncentiveType.AmountPerUom, 15, 5, true, true)]  // Zero amount
    [InlineData("R8", "P8", IncentiveType.AmountPerUom, 15, 0, SupportedIncentiveType.AmountPerUom, 15, 0, true, true)] // Zero volume
    public void Calculate_ShouldFail_UnderInvalidConditions(
        string rebateId, string productId, IncentiveType incentive, decimal rebateAmount, decimal percentage,
        SupportedIncentiveType productFlags, decimal productPrice, decimal volume, bool rebateExists, bool productExists)
    {
        // Arrange
        CalculateRebateRequest request = new() { RebateIdentifier = rebateId, ProductIdentifier = productId, Volume = volume };
        Rebate rebate = rebateExists ? new Rebate { Identifier = rebateId, Incentive = incentive, Amount = rebateAmount, Percentage = (decimal)percentage } : null;
        Product product = productExists ? new Product { Identifier = productId, SupportedIncentives = productFlags, Price = productPrice } : null;

        _mockRebateStore.Setup(x => x.GetRebate(rebateId)).Returns(rebate);
        _mockProductStore.Setup(x => x.GetProduct(productId)).Returns(product);

        // Act
        CalculateRebateResult result = _service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        _mockRebateStore.Verify(x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    // FixedCashAmount: Expects flat rebate amount (250m)
    [InlineData(IncentiveType.FixedCashAmount, 250, 0, SupportedIncentiveType.FixedCashAmount, 50, 1, 250)]
    // FixedRateRebate: Price (100) * Percentage (0.05) * Volume (20) = 100m
    [InlineData(IncentiveType.FixedRateRebate, 0, 0.05, SupportedIncentiveType.FixedRateRebate, 100, 20, 100)]
    // AmountPerUom: Rebate Amount (15) * Volume (5) = 75m
    [InlineData(IncentiveType.AmountPerUom, 15, 0, SupportedIncentiveType.AmountPerUom, 0, 5, 75)]
    public void Calculate_ShouldSucceed_AndStoreCorrectValues(
        IncentiveType incentive, decimal rebateAmount, decimal percentage,
        SupportedIncentiveType productFlags, decimal productPrice, decimal volume, decimal expectedResultAmount)
    {
        // Arrange
        CalculateRebateRequest request = new() { RebateIdentifier = "REB_OK", ProductIdentifier = "PROD_OK", Volume = volume };
        Rebate rebate = new() { Identifier = "REB_OK", Incentive = incentive, Amount = rebateAmount, Percentage = (decimal)percentage };
        Product product = new() { Identifier = "PROD_OK", SupportedIncentives = productFlags, Price = productPrice };

        _mockRebateStore.Setup(x => x.GetRebate("REB_OK")).Returns(rebate);
        _mockProductStore.Setup(x => x.GetProduct("PROD_OK")).Returns(product);

        // Act
        CalculateRebateResult result = _service.Calculate(request);

        // Assert
        Assert.True(result.Success);
        _mockRebateStore.Verify(x => x.StoreCalculationResult(rebate, expectedResultAmount), Times.Once);
    }
}
