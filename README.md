# ApexOverlay (iRacing Overlay)

A modern, clean iRacing overlay platform inspired by iOverlay.

## Current Test Mode (ready to test while running iRacing)

This repository now includes a local telemetry bridge endpoint plus a browser overlay UI:

- Bridge endpoint: `http://127.0.0.1:32123/telemetry`
- Health endpoint: `http://127.0.0.1:32123/health`
- Overlay UI: Vite app in `apps/overlay-ui`

You can run these side-by-side with iRacing on Windows to test overlay behavior, sizing, readability, and refresh cadence.

## Quick Start

1. Terminal A:
   - `cargo run --manifest-path services/iracing-bridge/Cargo.toml`
2. Terminal B:
   - `npm install`
   - `npm run dev`
3. Open the local Vite URL and place it on top of iRacing (borderless/windowed mode recommended for testing).

> Note: Telemetry is currently bridge-generated demo data for integration testing. Real iRacing SDK ingestion is the next step.

## Project Layout

```
apps/
  desktop/
  overlay-ui/
packages/
  telemetry-types/
  widget-sdk/
services/
  iracing-bridge/
```
