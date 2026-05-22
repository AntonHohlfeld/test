# Implementation Next Steps

## Vertical Slice Included
- Overlay host (`Overlay.App`) with transparent, topmost WPF window.
- Replay telemetry source (`Overlay.Telemetry`) emitting mock frames on a live loop.
- Processing layer (`Overlay.Processing`) for Relative/Fuel/Flags state projection.
- Domain interfaces/contracts (`Overlay.Domain`) including `ITelemetrySource` and `IWidget<TState>`.

## Immediate Follow-Up
1. Replace `ReplayTelemetrySource` with iRacing shared-memory adapter.
2. Add settings persistence (`settings.json`) for widget positions and click-through toggle.
3. Split UI into widget controls and add edit mode interactions.
4. Add disconnect/reconnect state machine with backoff and diagnostics logs.
