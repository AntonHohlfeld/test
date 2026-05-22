# Local Build Results (User-Reported Windows Environment)

## Environment
- .NET SDK: 8.0.421
- Runtimes include Microsoft.WindowsDesktop.App 8.0.27

## Reproduced Failure
`dotnet build IracingOverlay.sln -c Debug` failed with compile errors in widget projects:
- `CS0234` / `CS0246` for missing `Overlay.Processing`, `TelemetryProcessor`, and widget state types.

## Root Cause
Widget projects referenced `Overlay.Domain` but **did not reference `Overlay.Processing`**, while widget implementations import and use processing types.

## Fix Applied
Added `ProjectReference` to `../Overlay.Processing/Overlay.Processing.csproj` in:
- `src/Overlay.Widgets.Relative/Overlay.Widgets.Relative.csproj`
- `src/Overlay.Widgets.Fuel/Overlay.Widgets.Fuel.csproj`
- `src/Overlay.Widgets.Flags/Overlay.Widgets.Flags.csproj`

Also changed `Overlay.App` project SDK from `Microsoft.NET.Sdk.WindowsDesktop` to `Microsoft.NET.Sdk` to align with .NET 8 warning guidance while preserving WPF settings.
