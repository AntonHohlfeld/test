# iRacing Overlay App — Product Requirements & Technical Architecture

## 1) Product Requirements Document (PRD)

### 1.1 Product Vision
Build a production-grade in-game overlay for iRacing that delivers critical race information with minimal performance impact, high readability at racing speeds, and deep customization for different sim-racing setups.

### 1.2 Target Users
- **Primary:** Competitive iRacing drivers (road, oval, multiclass).
- **Secondary:** League racers, endurance teams, and streamers.

### 1.3 Success Metrics
- Overlay process average CPU usage < 3% on mid-range gaming CPUs while racing.
- RAM usage < 250 MB for standard widget set.
- UI update latency for critical values (relative/radar/flags) < 100 ms.
- Crash-free session rate > 99.5% over 2-hour runs.
- First-time setup under 10 minutes.

### 1.4 Platform & Constraints
- **OS:** Windows 10/11 (iRacing primary platform).
- **Integration:** iRacing telemetry via memory-mapped shared telemetry API and session data.
- **Operational Modes:** Borderless overlay with optional click-through.

### 1.5 Functional Requirements
#### Core
- Detect iRacing running/not running.
- Read telemetry/session data continuously.
- Render transparent overlay above game window.
- Drag/resize/toggle widgets in edit mode.
- Persist profiles/layouts/themes.

#### Widgets (Required)
1. Relative timing
2. Standings
3. Fuel
4. Lap timing
5. Tire/car status
6. Flags/race control
7. Track map/radar
8. Inputs
9. Session info
10. Spotter-style warnings

### 1.6 Non-Functional Requirements
- Startup under 2 seconds after launch.
- No blocking calls on render thread.
- Graceful degradation when data fields unavailable.
- Telemetry disconnect recovery without restart.
- All user config file-backed and human-readable.

---

## 2) Recommended Technical Architecture

### 2.1 Stack Decision
**Recommendation: C# + .NET 8 + WinUI 3 (or WPF fallback) with a native D3D-backed transparent overlay window.**

#### Why this over alternatives
- **Electron:** Fast UI iteration but higher memory/CPU overhead and poorer frame-time consistency for a real-time racing overlay.
- **Tauri:** Better than Electron for footprint, but still introduces web runtime complexity and less mature Windows overlay ergonomics for low-latency HUD rendering.
- **C++/Qt:** Excellent performance but significantly higher development complexity and slower product iteration for a customizable UI-heavy app.
- **C#/WPF/WinUI:** Strong Windows APIs, great developer velocity, broad telemetry ecosystem libraries, easy packaging/updating, and enough performance with disciplined rendering.

**Practical choice:**
- **MVP:** WPF + D3DImage/optimized retained-mode controls.
- **v1+:** WinUI 3 + Windows App SDK with composition optimizations if needed.

### 2.2 High-Level Modules
1. **Telemetry Service**
   - Reads iRacing shared memory snapshots.
   - Emits immutable telemetry frames.
2. **Data Processing Layer**
   - Computes derived metrics (fuel/lap deltas/gaps/radar states).
   - Applies smoothing/debouncing where needed.
3. **Overlay Renderer**
   - Owns transparent topmost window and render scheduling.
4. **Widget System**
   - Pluggable widget contracts and lifecycle.
5. **Settings/Configuration**
   - JSON profiles, per-car/per-track presets.
6. **Theme System**
   - Tokenized colors/spacing/typography and accessibility presets.
7. **Logging/Error Handling**
   - Structured logs + rolling files + telemetry health events.
8. **Performance Monitoring**
   - Internal perf counters (render time, update backlog, dropped frames).

---

## 3) Feature Ranking (MVP / v1 / Advanced)

### MVP (ship quickly, high impact)
- Telemetry connection manager + reconnect logic.
- Relative timing, fuel, lap timing, flags, inputs, session info widgets.
- Basic radar (left/right proximity bands).
- Edit mode: move/resize/enable/disable widgets.
- One dark theme + one high-contrast theme.
- JSON settings profiles.

### v1
- Full standings widget with multiclass sorting.
- Track map with nearby cars.
- Tire/car status expanded (temps, brake bias, limited damage signals).
- Spotter-style visual alerts (car left/right, low fuel, pit window).
- Multiple saved layouts by resolution/setup.
- Hotkeys and click-through toggle.

### Advanced
- Voice alert engine and custom alert packs.
- Strategy assistant (stint/fuel/yellow projections).
- Team endurance shared session sync.
- Cloud profile backup.
- Replay mode analytics and post-race review overlays.

---

## 4) Suggested Folder / Project Structure

```text
iracing-overlay/
  src/
    Overlay.App/                # Entry, windowing, DI bootstrapping
    Overlay.Telemetry/          # iRacing shared memory adapters
    Overlay.Domain/             # Core models/interfaces/events
    Overlay.Processing/         # Derived metrics, calculators, filters
    Overlay.Widgets/
      Relative/
      Standings/
      Fuel/
      LapTiming/
      TireCar/
      Flags/
      Radar/
      Inputs/
      Session/
      Alerts/
    Overlay.Rendering/          # Render scheduler/compositor hooks
    Overlay.Settings/           # Profile schema, persistence, migration
    Overlay.Theming/            # Theme tokens/styles
    Overlay.Diagnostics/        # Logging, perf counters, health checks
  tests/
    Overlay.UnitTests/
    Overlay.IntegrationTests/
    Overlay.LoadTests/
  tools/
    TelemetryCapture/           # Record/replay telemetry streams
  docs/
    PRD.md
    ARCHITECTURE.md
    WIDGET_SPECS.md
```

---

## 5) Data Flow Explanation

1. **Telemetry Poller** reads shared memory snapshots at high frequency.
2. **Frame Normalizer** validates timestamps/session transitions.
3. **Processing Pipelines** compute derived domain objects:
   - Relative positions, class context, gaps.
   - Fuel burn rolling average and pit recommendation.
   - Lap/sector deltas and optimal estimate.
   - Proximity/radar hazard states.
4. **State Store** keeps latest immutable state + short rolling history.
5. **Widget Update Bus** pushes only changed slices to each widget.
6. **Renderer** invalidates only dirty regions/widgets.

Key principle: **separate ingestion rate from rendering rate** so high-frequency telemetry does not force full UI redraw every sample.

---

## 6) UI Layout Description

### Design Principles
- Bold typography, high contrast, low visual clutter.
- Information hierarchy: safety-critical > strategy > informational.
- Color semantics consistent (green good, amber caution, red urgent, blue informational).

### Example Layouts
#### Single Monitor (16:9)
- Top-left: relative
- Top-right: session + flags
- Bottom-right: fuel + lap timing
- Bottom-center: inputs
- Mid-left/right: radar proximity bars

#### Ultrawide (21:9 / 32:9)
- Side gutters host standings and tire/car status.
- Central lower area for inputs and lap delta.
- Compact radar near center for peripheral readability.

#### Triple Monitor
- Critical widgets confined to center monitor safe zone.
- Lower-priority standings/session details on left/right monitors.

#### VR-Friendly
- Minimal floating panel anchored low center.
- Large text relative + fuel + alerts only by default.
- Avoid dense standings tables in main gaze path.

---

## 7) Widget Specifications (Condensed)

### Relative Timing
- Inputs: player idx, nearby cars, lap distance, class, pit state.
- Refresh: 100 ms.
- Alerts: closing rate thresholds.

### Standings
- Inputs: session results + live order.
- Refresh: 500 ms.
- Sorting modes: overall/class.

### Fuel
- Inputs: fuel level, consumption history, lap times.
- Refresh: 500 ms (core), 1 s for recommendations.

### Lap Timing
- Inputs: lap/sector times, best/last/current delta.
- Refresh: 100 ms for delta, 500 ms for summary values.

### Tire & Car Status
- Inputs: tire temps/wear (if available), brake bias, engine temps, damage proxies.
- Refresh: 500 ms.

### Flags / Race Control
- Inputs: current flag and penalties.
- Refresh: event-driven + 100 ms polling fallback.

### Radar / Track Map
- Inputs: nearby car vectors + track position.
- Refresh: 60 Hz equivalent for close proximity logic.

### Inputs
- Inputs: throttle/brake/clutch/steering/gear/rpm/speed.
- Refresh: 60 Hz (or iRacing update cadence bound).

### Session Info
- Inputs: session type, time/laps remaining, weather temps.
- Refresh: 1 s.

### Spotter Warnings
- Inputs: radar + race control + fuel strategy states.
- Refresh: event-driven, cooldown-debounced.

---

## 8) Performance Optimization Plan

### Update Cadence Strategy
- **Per frame / 60 Hz:** inputs, close radar hazard states.
- **Every 100 ms:** relative gaps, lap delta, flags fallback checks.
- **Every 500 ms:** standings, fuel burn rolling calc, tire/car status.
- **Every 1 s:** session timers, weather, non-critical stats.

### Rendering Optimization
- Dirty-rectangle redraw only.
- Widget-level throttling (skip identical state renders).
- Pre-render static assets/icons.
- Use GPU-accelerated primitives for gauges/bars.
- Limit transparency layers and blur effects.

### CPU/Memory Optimization
- Allocate object pools for hot-path telemetry transforms.
- Use struct-like immutable snapshots for hot data.
- Ring buffers for history windows (avoid unbounded lists).
- Lock-free queues/channels between poller and UI.

### Resilience
- Detect stale telemetry timestamp; switch widgets to “paused/disconnected” state.
- Auto-reconnect backoff (0.5s, 1s, 2s, capped).
- Never block render loop on telemetry read failures.

---

## 9) Development Roadmap

### Phase 0 (1-2 weeks)
- Repo scaffolding, architecture skeleton, telemetry capture/replay tool.
- Basic overlay window + click-through toggle.

### Phase 1 MVP (3-5 weeks)
- Telemetry service stable ingestion.
- Core widgets: relative, fuel, lap timing, flags, inputs, session.
- Edit mode + profile save/load.
- Basic diagnostics panel.

### Phase 2 v1 (4-6 weeks)
- Standings full implementation.
- Radar/track map production-ready.
- Tire/car status and richer alerts.
- Theme editor and layout presets.

### Phase 3 Polish (ongoing)
- Load testing, memory profiling, UX iteration from beta racers.
- Installer/updater, crash reporting, docs/videos.

---

## 10) Risks & Technical Challenges

- **Telemetry field availability variance:** some values differ by car/session; need fallback states.
- **Overlay compatibility edge cases:** fullscreen exclusive modes and anti-cheat sensitivities.
- **Overdraw/perf regressions:** too many animated widgets can spike frame times.
- **Data correctness under cautions/pit cycles:** ordering and gap logic can be tricky.
- **VR ergonomics:** minimizing distraction while preserving utility.

Mitigation: telemetry replay tests, feature flags per widget, strict perf budgets in CI load tests.

---

## 11) Example Pseudocode

### Telemetry Loop
```pseudo
init telemetryReader
init channel<TelemetryFrame>(bounded=3)

task TelemetryIngestLoop:
  while appRunning:
    if !telemetryReader.isConnected():
      telemetryReader.tryConnect()
      sleep(250ms)
      continue

    raw = telemetryReader.readSnapshot()
    if raw is invalid:
      publish HealthEvent(Degraded)
      sleep(16ms)
      continue

    frame = normalize(raw)
    tryWriteLatest(channel, frame)   # drop oldest when full
    sleep(16ms)                      # ~60Hz ingest target
```

### Processing + Widget Update Scheduler
```pseudo
task ProcessingLoop:
  every 16ms:
    frame = channel.readLatestOrNull()
    if frame != null:
      coreState = computeFastState(frame)         # inputs/radar
      stateStore.update(coreState)

  every 100ms:
    frame = stateStore.latestFrame()
    relative = calcRelative(frame)
    lapDelta = calcLapDelta(frame)
    flags = calcFlags(frame)
    publishWidgetUpdates([relative, lapDelta, flags])

  every 500ms:
    frame = stateStore.latestFrame()
    standings = calcStandings(frame)
    fuel = calcFuelProjection(frame, history)
    carStatus = calcCarStatus(frame)
    publishWidgetUpdates([standings, fuel, carStatus])

  every 1s:
    frame = stateStore.latestFrame()
    session = calcSessionInfo(frame)
    publishWidgetUpdates([session])

UI Render Loop:
  onWidgetUpdate(update):
    if update.differsFromPrevious:
      markWidgetDirty(update.widgetId)

  onRenderTick:
    redrawDirtyWidgetsOnly()
```

---

## 12) Recommended Next Steps (Implementation Start)

1. Lock stack decision to **.NET 8 + WPF** for fastest MVP path.
2. Build telemetry adapter first with replay capability (critical for deterministic testing).
3. Implement widget SDK contract (`IWidget`, `IWidgetState`, `IWidgetRenderer`).
4. Deliver vertical slice: Relative + Fuel + Flags in overlay window.
5. Add edit mode/layout persistence before adding more widgets.
6. Add perf HUD early (frame time, update queue lag, memory).
7. Recruit 5-10 sim racers for weekly feedback loops.

This sequence minimizes risk, proves performance early, and prevents overbuilding before core usability is validated.
