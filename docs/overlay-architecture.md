# Overlay Architecture

## 1) Core Principles

1. **Low latency first**: telemetry pipeline should prioritize deterministic update timing.
2. **Composable widgets**: each widget receives a strongly typed data slice + event bus.
3. **Safe-by-default UX**: clean defaults, legible at glance, minimal distraction.
4. **Recoverable runtime**: every widget can crash/reload independently without full app exit.

## 2) Runtime Model

- A **host process** manages lifecycle, auth, profiles, persistence, and tray controls.
- A **telemetry bridge service** ingests iRacing SDK data, normalizes, and publishes events.
- Multiple **overlay windows** subscribe to event channels and render widget trees.

## 3) Data Flow

1. Bridge polls iRacing SDK (or listens to session updates where available).
2. Normalizer maps raw SDK fields to `TelemetryFrame` and domain events.
3. Event router fans out to subscribed widgets.
4. Widgets run selectors with memoization and render at capped FPS.

## 4) Windowing + Overlay Behaviors

- Frameless transparent windows.
- Always-on-top with click-through toggle.
- Per-widget lock/unlock for drag and resize.
- Scene profiles (Practice / Quali / Race / Broadcast).

## 5) Performance Targets (MVP)

- Telemetry end-to-end update budget: **<= 40ms** typical.
- UI frame rate target: **60 FPS** for lightweight widgets.
- App idle memory target: **< 350 MB** total.

## 6) Reliability + Observability

- Structured logging with levels and per-widget tags.
- Crash boundaries for each widget.
- Health panel: telemetry freshness, dropped frames, CPU usage.

## 7) Security + Safety

- No external command execution from widget layer.
- Signed release artifacts.
- Optional cloud sync disabled by default.

## 8) Extensibility

- `WidgetManifest` contract:
  - id, name, version, min_host_version
  - settings schema
  - required telemetry channels
- Versioned event schema for backward compatibility.
