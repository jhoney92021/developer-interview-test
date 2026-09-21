# Smartwyre Developer Test - Architectural Overview

### Navigation Links

| Return to Main | Configuration Guides | Terminal Automation |
| :--- | :--- | :--- |
| [Main Readme](Readme.md) | [New Incentive Runbook](new-incentive.md) | [CLI Cheat Sheet](cheat-sheet.md) |

## Architecture & Refactoring Decisions

### 1. Constructor-Injected Dependency Isolation (SOLID)
* **Dependency Inversion Principle (DIP):** Refactored `RebateService` to accept dependencies directly through its constructor using domain abstractions (`IRebateDataStore`, `IProductDataStore`, and `ILogger<RebateService>`). This explicitly declares service requirements up front and prevents tight coupling.
* **Single Responsibility Principle (SRP):** Database connection lifecycles, file handling, and logging implementations are decoupled from the data store interfaces. Data stores focus exclusively on record persistence, leaving connection lifecycle management to the consuming application layer.
* **Interface Segregation Principle (ISP):** The service interacts through granular, isolated storage contracts rather than exposing a monolithic database context payload.

### 2. Pragmatic Domain Engine & Modern C# (YAGNI Focus)
* **Domain Extension Strategy:** Consolidated input domain checking, business validation rules, and mathematical formulas into isolated extension methods inside `IncentiveTypeExtensions.cs`. This prevents the core `RebateService` from becoming anemic while decoupling data extraction workflows from pure mathematical execution paths.
* **Pattern Matching Expressions:** Replaced legacy nested conditional syntax blocks with clean, declarative, modern C# switch expressions to drive calculation and input verification trees.
* **Enum Safety Mapping:** Implemented an explicit `ToSupportedFlag()` switch expression to translate single `IncentiveType` items over to bitmask-compatible `SupportedIncentiveType` flags. This safeguards validation math and eliminates fragile implicit integer index tracking errors.
* **YAGNI Alignment:** Opted for a streamlined, data-driven domain extension architecture over an over-engineered multi-class Strategy Pattern footprint. This fulfills the structural requirements of the prompt perfectly without bloating the workspace with redundant abstraction boilerplate.

### 3. Integrated QA & Test Automation Footprint
* **Mock-Isolated Unit Tests:** Configured inside `RebateServiceTests.cs`. Leverages `Moq` and explicit typing to execute boundary test permutations in microseconds without requiring disk lookups or database connection overhead.
* **Living Documentation Theories:** Implemented xUnit `[Theory]` suites targeting the underlying calculation extension blocks. This transforms the testing layer into descriptive, readable, parameter-driven specification tables.
* **Process-Driven Integration Tests:** Located inside `RunnerIntegrationTests.cs`. Launches full process pipelines using diagnostic environment contexts to validate argument parsing, runtime schema generation, and physical SQLite database transactions.

---

## Visual Studio Test Cache Warning

If you are running the `RunnerIntegrationTests` suite inside **Visual Studio's Test Explorer** and encounter unexpected `SqliteException` or `DbUpdateException` table mapping errors (even after modifying schema files or deleting `smartwyre.db` manually), you are experiencing a Visual Studio shadow-copy caching defect.

Visual Studio frequently virtualizes integration test execution scopes inside isolated target artifact environments, caching and copying old database binaries with outdated schemas instead of instantiating fresh schemas from code changes on disk.

### The Fix

To break through the IDE build cache and force a complete environment synchronization, execute the following compound command directly from your terminal root:

```powershell
dotnet clean; Remove-Item -Path "**/bin", "**/obj" -Recurse -Force -ErrorAction SilentlyContinue; dotnet test
```

This script safely nukes all virtualized compilation caches, forces Visual Studio to completely recompile your workspace from scratch, and cleanly invokes the SQLite schema auto-initialization engine.
