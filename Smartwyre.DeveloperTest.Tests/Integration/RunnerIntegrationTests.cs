using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Integration;

public class RunnerIntegrationTests
{
    private static string GetRunnerProjectDirectory()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        return Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "Smartwyre.DeveloperTest.Runner"));
    }

    private static ProcessStartInfo CreateProcessStartInfo(string rebateCode, string productIdentifier, string volume)
    {
        string runnerProjectDir = GetRunnerProjectDirectory();
        return new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{runnerProjectDir}\" -- \"{rebateCode}\" \"{productIdentifier}\" \"{volume}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    }

    [Fact]
    public void ExecuteRunner_WithFixedCashAmountScenario_ReturnsSuccessOutput()
    {
        ProcessStartInfo startInfo = CreateProcessStartInfo("REBATE_FIXED_CASH", "PROD_FIXED_CASH", "1");

        using (Process process = Process.Start(startInfo))
        {
            Assert.NotNull(process);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.Equal(0, process.ExitCode);
            Assert.True(string.IsNullOrEmpty(error), $"Execution error: {error}");
            Assert.Contains("Result Success Status: True", output);
        }
    }

    [Fact]
    public void ExecuteRunner_WithFixedRateScenario_ReturnsSuccessOutput()
    {
        ProcessStartInfo startInfo = CreateProcessStartInfo("REBATE_FIXED_RATE", "PROD_FIXED_RATE", "10");

        using (Process process = Process.Start(startInfo))
        {
            Assert.NotNull(process);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.Equal(0, process.ExitCode);
            Assert.True(string.IsNullOrEmpty(error), $"Execution error: {error}");
            Assert.Contains("Result Success Status: True", output);
        }
    }

    [Fact]
    public void ExecuteRunner_WithAmountPerUomScenario_ReturnsSuccessOutput()
    {
        ProcessStartInfo startInfo = CreateProcessStartInfo("REBATE_UOM", "PROD_UOM", "5");

        using (Process process = Process.Start(startInfo))
        {
            Assert.NotNull(process);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.Equal(0, process.ExitCode);
            Assert.True(string.IsNullOrEmpty(error), $"Execution error: {error}");
            Assert.Contains("Result Success Status: True", output);
        }
    }

    [Fact]
    public void ExecuteRunner_WithMismatchedIncentiveTypes_ReturnsFailureOutput()
    {
        ProcessStartInfo startInfo = CreateProcessStartInfo("REBATE_FIXED_RATE", "PROD_FIXED_CASH", "10");

        using (Process process = Process.Start(startInfo))
        {
            Assert.NotNull(process);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.Equal(0, process.ExitCode);
            Assert.Contains("Result Success Status: False", output);
        }
    }
}
