using System;

namespace Smartwyre.DeveloperTest.Types;

public class RebateCalculationLog
{
    // Unique identifier for the log record entry in SQLite
    public int Id { get; set; }

    // Identifies which rebate rules were applied
    public string RebateIdentifier { get; set; }

    // The final calculated monetary output stored by the engine
    public decimal Amount { get; set; }

    // Automatically stamps the exact time the row was logged
    public DateTime CalculatedDateTime { get; set; } = DateTime.UtcNow;
}
