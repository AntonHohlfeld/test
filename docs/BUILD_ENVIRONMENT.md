# Build Environment Notes

In this container, the .NET SDK is not installed, so local compile validation cannot run here.

## Observed Command Output
- `dotnet build IracingOverlay.sln`
- Output: `/bin/bash: line 1: dotnet: command not found`

## Developer Action
Run the following on a machine with .NET 8 SDK installed:

```bash
dotnet --info
dotnet restore IracingOverlay.sln
dotnet build IracingOverlay.sln -c Release
```


## Interpretation for CI/Agent Reports
- Treat this build result as a **warning** caused by missing tooling in the container.
- Do not mark it as a code regression unless `dotnet` is present and build fails.
