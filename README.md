# ApexOverlay (iRacing Overlay)

A modern, clean, and actively maintained iRacing overlay platform inspired by iOverlay, built with a modular architecture so each widget can evolve independently.

## Vision

Create a complete overlay suite that is:
- **Modern**: smooth animations, theme system, responsive layouts.
- **Clean**: unobtrusive default UI, readable typography, high-contrast telemetry.
- **Updated**: maintainable codebase, typed contracts, test automation, and release pipelines.

## MVP Widgets

- Relative
- Standings
- Input telemetry (throttle/brake/steering)
- Fuel calculator + stint prediction
- Track map + nearby cars
- Spotter/race-control notifications

## Target Architecture

- **Desktop Host**: Tauri (Windows-first) for low overhead and tray integration.
- **UI Layer**: React + TypeScript + Tailwind + shadcn/ui primitives.
- **Data Layer**: Rust service consuming iRacing SDK data and exposing typed events to the UI.
- **Overlay Runtime**: frameless always-on-top windows with per-widget positioning and scene profiles.
- **Persistence**: SQLite for settings/profile/widget state snapshots.

See [`docs/overlay-architecture.md`](docs/overlay-architecture.md) for full details and [`docs/product-roadmap.md`](docs/product-roadmap.md) for milestones.

## Proposed Monorepo Layout

```
apps/
  desktop/        # Tauri shell + window manager
  overlay-ui/     # React widgets + design system
packages/
  telemetry-types/# shared contracts
  widget-sdk/     # widget plugin helpers
services/
  iracing-bridge/ # Rust iRacing SDK adapter
```

## Getting Started (Scaffold Phase)

This repository currently contains product and architecture specs to align scope and implementation approach before coding begins.

Next implementation step:
1. Scaffold `apps/overlay-ui` with Vite + React + TypeScript.
2. Scaffold `apps/desktop` with Tauri and multi-window control.
3. Implement `services/iracing-bridge` event stream and mock replay mode.

