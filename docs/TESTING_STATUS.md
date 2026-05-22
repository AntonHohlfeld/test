# Testing Status and Reporting Rules

This repository is currently being edited in an environment where the .NET SDK is unavailable.

## Current Environment Fact
- Command: `dotnet build IracingOverlay.sln`
- Actual output: `/bin/bash: line 1: dotnet: command not found`

## Interpretation
- This is an **environment limitation**, not a code failure.
- Report this check with a warning marker (`⚠️`) rather than a failure marker (`❌`).

## Recommended Verification on a Dev Machine
Run on Windows (or any machine with .NET 8 SDK installed):

```bash
dotnet --info
dotnet restore IracingOverlay.sln
dotnet build IracingOverlay.sln -c Release
```
