import React from 'react';
import { createRoot } from 'react-dom/client';

function App() {
  return (
    <main style={{ fontFamily: 'Inter, sans-serif', padding: 16, color: '#e6edf3', background: '#0b1220', minHeight: '100vh' }}>
      <h1>ApexOverlay</h1>
      <p>Overlay UI scaffold is running.</p>
    </main>
  );
}

createRoot(document.getElementById('root')!).render(<App />);
