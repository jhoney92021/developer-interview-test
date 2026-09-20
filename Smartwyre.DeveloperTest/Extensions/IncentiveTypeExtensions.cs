using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Extensions;

/// <summary>
/// Provides domain-centric extension methods for the <see cref="IncentiveType"/> enum 
/// to encapsulate validation mapping and mathematical calculation business rules.
/// </summary>
public static class IncentiveTypeExtensions
{
    /// <summary>
    /// Computes the final monetary rebate amount based on the rules specified by the incentive type.
    /// </summary>
    /// <param name="incentive">The active incentive calculation rule type.</param>
    /// <param name="rebate">The rebate configuration record supplying target rates or flat amounts.</param>
    /// <param name="product">The product record containing structural details like base price.</param>
    /// <param name="volume">The operational quantity or volume supplied in the calculation request.</param>
    /// <returns>
    /// A <see cref="decimal"/> indicating the evaluated rebate amount. 
    /// Returns <c>0m</c> if an unrecognized or unmapped incentive type is evaluated.
    /// </returns>
    public static decimal CalculateAmount(this IncentiveType incentive, Rebate rebate, Product product, decimal volume) =>
        incentive switch
        {
            IncentiveType.FixedCashAmount => rebate.Amount,
            IncentiveType.FixedRateRebate => product.Price * rebate.Percentage * volume,
            IncentiveType.AmountPerUom => rebate.Amount * volume,
            _ => 0m
        };

    /// <summary>
    /// Validates that all critical numeric requirements (such as non-zero values for volume, 
    /// price, percentage, or amount configurations) are satisfied for the given incentive rule block.
    /// </summary>
    /// <param name="incentive">The active incentive calculation rule type.</param>
    /// <param name="rebate">The rebate configuration record to validate.</param>
    /// <param name="product">The product record to validate.</param>
    /// <param name="volume">The transactional volume or quantity to validate.</param>
    /// <returns>
    /// <see langword="true"/> if all necessary mathematical properties are safely non-zero 
    /// and present; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool HasValidInputs(this IncentiveType incentive, Rebate rebate, Product product, decimal volume) =>
        incentive switch
        {
            IncentiveType.FixedCashAmount => rebate.Amount != 0,
            IncentiveType.FixedRateRebate => rebate.Percentage != 0 && product.Price != 0 && volume != 0,
            IncentiveType.AmountPerUom => rebate.Amount != 0 && volume != 0,
            _ => false
        };

    /// <summary>
    /// Maps a single <see cref="IncentiveType"/> option over to its corresponding bitmask-compatible 
    /// representation in <see cref="SupportedIncentiveType"/> to prevent order-dependency defects.
    /// </summary>
    /// <param name="incentive">The single input incentive type to map.</param>
    /// <returns>The matching bit flag value used to evaluate product compatibility masks.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if an undefined integer or unsupported enum item is evaluated.
    /// </exception>
    public static SupportedIncentiveType ToSupportedFlag(this IncentiveType incentive) =>
        incentive switch
        {
            IncentiveType.FixedRateRebate => SupportedIncentiveType.FixedRateRebate,
            IncentiveType.AmountPerUom => SupportedIncentiveType.AmountPerUom,
            IncentiveType.FixedCashAmount => SupportedIncentiveType.FixedCashAmount,
            _ => throw new ArgumentOutOfRangeException(nameof(incentive), incentive, "Unsupported incentive type encountered.")
        };
}
