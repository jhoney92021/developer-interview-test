# Adding a New Incentive Type - Runbook

### Navigation Links

| Return to Main | Configuration Guides | Terminal Automation |
| :--- | :--- | :--- |
| [Main Readme](Readme.md) | [New Incentive Runbook](new-incentive.md) | [CLI Cheat Sheet](cheat-sheet.md) |

Because the calculation engine logic has been decoupled from database layers and service orchestration steps, adding a brand new incentive type tomorrow requires **zero modifications** to `RebateService.cs`, your repository configurations, or database entities. 

To extend the system, follow these three simple steps:

### Step 1: Update the Core Enums
Add your new incentive variant to the shared enum definitions inside the types project (`Smartwyre.DeveloperTest.Types`):

```csharp
public enum IncentiveType
{
    // ... existing types
    NewIncentiveType 
}

[Flags]
public enum SupportedIncentiveType
{
    // ... existing flags
    NewIncentiveType = 1 << 3
}
```

### Step 2: Register the Map and Calculation Expressions
Open `IncentiveTypeExtensions.cs` and add a new row mapping your variant inside the three existing modern switch expressions:

```csharp
public static class IncentiveTypeExtensions
{
    public static SupportedIncentiveType ToSupportedFlag(this IncentiveType incentive) =>
        incentive switch
        {
            // ... existing mappings
            IncentiveType.NewIncentiveType => SupportedIncentiveType.NewIncentiveType,
            _ => (SupportedIncentiveType)0 
        };

    public static bool HasValidInputs(this IncentiveType incentive, Rebate rebate, Product product, decimal volume) =>
        incentive switch
        {
            // ... existing validations
            IncentiveType.NewIncentiveType => rebate.Amount != 0 && product.Price != 0,
            _ => false
        };

    public static decimal CalculateAmount(this IncentiveType incentive, Rebate rebate, Product product, decimal volume) =>
        incentive switch
        {
            // ... existing formulas
            IncentiveType.NewIncentiveType => rebate.Amount + (product.Price * 0.05m),
            _ => 0m
        };
}
```

### Step 3: Add Living Documentation Test Cases
Open `IncentiveTypeExtensionsTests.cs` and add your mathematical edge cases right into the existing xUnit `[Theory]` blocks as a simple `[InlineData]` row. This ensures your new business math is instantly tested and documented in microseconds without spinning up external dependencies:

```csharp
[Theory]
// Existing data blocks...
[InlineData(IncentiveType.NewIncentiveType, 50, 100, 1, 55)] // 50 (Amount) + (100 * 0.05) = 55
public void CalculateAmount_ShouldReturnExpectedValue_ForAllIncentiveTypes(...)
{
    // Executed automatically alongside existing test matrices
}
```
