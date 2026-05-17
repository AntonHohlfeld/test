import { createRoot } from 'react-dom/client';
import { useEffect, useMemo, useState } from 'react';
import type { TelemetryFrame } from '@apex/telemetry-types';

type OverlayFrame = TelemetryFrame & {
  position: number;
  lapsRemaining: number;
};

const FALLBACK: OverlayFrame = {
  speedKph: 0,
  fuelLiters: 42,
  throttle: 0,
  brake: 0,
  position: 12,
  lapsRemaining: 28,
};

function pct(n: number): string {
  return `${Math.round(n * 100)}%`;
}

function Bar({ label, value, color }: { label: string; value: number; color: string }) {
  return <section style={{ marginBottom: 10 }}><div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}><span>{label}</span><strong>{pct(value)}</strong></div><div style={{ height: 8, background: '#1f2937', borderRadius: 999 }}><div style={{ height: 8, width: pct(value), background: color, borderRadius: 999, transition: 'width 120ms linear' }} /></div></section>;
}

function App() {
  const [frame, setFrame] = useState<OverlayFrame>(FALLBACK);
  const [bridgeOk, setBridgeOk] = useState(false);

  useEffect(() => {
    let mounted = true;

    const poll = async () => {
      try {
        const res = await fetch('http://127.0.0.1:32123/telemetry');
        if (!res.ok) throw new Error(`bridge http ${res.status}`);
        const data = (await res.json()) as OverlayFrame;
        if (mounted) {
          setFrame(data);
          setBridgeOk(true);
        }
      } catch {
        if (mounted) setBridgeOk(false);
      }
    };

    poll();
    const id = setInterval(poll, 120);
    return () => {
      mounted = false;
      clearInterval(id);
    };
  }, []);

  const estLapsLeft = useMemo(() => frame.fuelLiters / 2.35, [frame.fuelLiters]);

  return <main style={{ fontFamily: 'Inter, ui-sans-serif, system-ui', padding: 16, color: '#e5e7eb', background: 'transparent', minHeight: '100vh' }}>
    <div style={{ maxWidth: 460, border: '1px solid #1f2937', borderRadius: 14, padding: 14, background: 'rgba(17,24,39,0.80)' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 12 }}>
        <h1 style={{ margin: 0, fontSize: 20 }}>ApexOverlay</h1>
        <span style={{ color: bridgeOk ? '#22c55e' : '#f59e0b' }}>{bridgeOk ? 'Bridge Connected' : 'Bridge Offline'}</span>
      </header>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 8, marginBottom: 12 }}>
        <div><div style={{ color: '#9ca3af', fontSize: 12 }}>Speed</div><strong>{Math.round(frame.speedKph)} kph</strong></div>
        <div><div style={{ color: '#9ca3af', fontSize: 12 }}>Position</div><strong>P{frame.position}</strong></div>
        <div><div style={{ color: '#9ca3af', fontSize: 12 }}>Fuel</div><strong>{frame.fuelLiters.toFixed(1)} L</strong></div>
      </div>
      <Bar label="Throttle" value={frame.throttle} color="#22c55e" />
      <Bar label="Brake" value={frame.brake} color="#ef4444" />
      <footer style={{ marginTop: 12, fontSize: 13, color: '#cbd5e1' }}>Est. laps left: <strong>{estLapsLeft.toFixed(1)}</strong> · Session laps remaining: <strong>{frame.lapsRemaining}</strong></footer>
    </div>
  </main>;
}

createRoot(document.getElementById('root')!).render(<App />);
