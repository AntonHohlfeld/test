import { createRoot } from 'react-dom/client';
import { useEffect, useMemo, useState } from 'react';
import type { TelemetryFrame } from '@apex/telemetry-types';

type OverlayFrame = TelemetryFrame & {
  position: number;
  lapsRemaining: number;
};

function pct(n: number): string {
  return `${Math.round(n * 100)}%`;
}

function Bar({ label, value, color }: { label: string; value: number; color: string }) {
  return (
    <section style={{ marginBottom: 10 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
        <span>{label}</span>
        <strong>{pct(value)}</strong>
      </div>
      <div style={{ height: 8, background: '#1f2937', borderRadius: 999 }}>
        <div style={{ height: 8, width: pct(value), background: color, borderRadius: 999, transition: 'width 120ms linear' }} />
      </div>
    </section>
  );
}

function App() {
  const [frame, setFrame] = useState<OverlayFrame>({ speedKph: 0, fuelLiters: 42, throttle: 0, brake: 0, position: 12, lapsRemaining: 28 });

  useEffect(() => {
    const id = setInterval(() => {
      setFrame((prev) => {
        const throttle = Math.max(0, Math.min(1, prev.throttle + (Math.random() - 0.45) * 0.25));
        const brake = Math.max(0, Math.min(1, prev.brake + (Math.random() - 0.6) * 0.25));
        const speedKph = Math.max(0, Math.min(315, prev.speedKph + (throttle - brake) * 17 + (Math.random() - 0.5) * 4));
        const fuelLiters = Math.max(0, prev.fuelLiters - 0.015 - speedKph / 100000);
        return { ...prev, throttle, brake, speedKph, fuelLiters };
      });
    }, 120);

    return () => clearInterval(id);
  }, []);

  const estLapsLeft = useMemo(() => frame.fuelLiters / 2.35, [frame.fuelLiters]);

  return <main style={{ fontFamily: 'Inter, ui-sans-serif, system-ui', padding: 16, color: '#e5e7eb', background: '#030712', minHeight: '100vh' }}>{/* trimmed */}
    <div style={{ maxWidth: 460, border: '1px solid #1f2937', borderRadius: 14, padding: 14, background: 'rgba(17,24,39,0.8)' }}>
      <h1 style={{ margin: '0 0 12px 0', fontSize: 20 }}>ApexOverlay</h1>
      <Bar label="Throttle" value={frame.throttle} color="#22c55e" />
      <Bar label="Brake" value={frame.brake} color="#ef4444" />
      <footer>Speed {Math.round(frame.speedKph)} kph · Fuel {frame.fuelLiters.toFixed(1)} L · Laps left {estLapsLeft.toFixed(1)}</footer>
    </div>
  </main>;
}

createRoot(document.getElementById('root')!).render(<App />);
