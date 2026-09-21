# Cheat Sheet

### Navigation Links

| Return to Main | Configuration Guides | Terminal Automation |
| :--- | :--- | :--- |
| [Main Readme](Readme.md) | [New Incentive Runbook](new-incentive.md) | [CLI Cheat Sheet](cheat-sheet.md) |
---
### Run Application (Interactive Mode)
Clears cached database binaries, flushes compilation directories, and launches the runner terminal interface:
```powershell
Remove-Item -Path ".\smartwyre.db", ".\Smartwyre.DeveloperTest.Runner\smartwyre.db" -ErrorAction SilentlyContinue; dotnet clean; Remove-Item -Path "**/bin", "**/obj" -Recurse -Force -ErrorAction SilentlyContinue; dotnet run --project Smartwyre.DeveloperTest.Runner
```
---
### Run Application (Argument Mode)
Wipes local environments and passes explicit seeded dataset identifiers straight down via CLI terminal arguments:
```powershell
Remove-Item -Path ".\smartwyre.db", ".\Smartwyre.DeveloperTest.Runner\smartwyre.db" -ErrorAction SilentlyContinue; dotnet clean; Remove-Item -Path "**/bin", "**/obj" -Recurse -Force -ErrorAction SilentlyContinue; dotnet run --project Smartwyre.DeveloperTest.Runner -- "REBATE_FIXED_CASH" "PROD_FIXED_CASH" "100.50"
```
---
### Fresh Workspace Build
Evicts lingering artifact files and triggers a clean solution compilation step from absolute scratch:
```powershell
dotnet clean; Remove-Item -Path "**/bin", "**/obj" -Recurse -Force -ErrorAction SilentlyContinue; dotnet build
```
---
## Testing
---
### Execute Verification Pipeline
Purges shadow-copy directories to bypass IDE caching quirks and fires off both unit and integration suites:
```powershell
dotnet clean; Remove-Item -Path "**/bin", "**/obj" -Recurse -Force -ErrorAction SilentlyContinue; dotnet test
```
