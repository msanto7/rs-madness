import { useEffect, useState } from 'react'

interface HealthStatus {
  status: string;
  database: string;
  userCount: number;
  timestamp: string;
}

function App() {
  const [health, setHealth] = useState<HealthStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch('http://localhost:5202/api/health')
      .then(res => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
      })
      .then(data => setHealth(data))
      .catch(err => setError(err.message));
  }, []);

  return (
    <div style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
      <h1>RS Madness — Connection Test</h1>

      {error && (
        <div style={{ color: 'red' }}>
          <strong>Error:</strong> {error}
        </div>
      )}

      {health && (
        <table style={{ borderCollapse: 'collapse', marginTop: '1rem' }}>
          <tbody>
            {Object.entries(health).map(([key, value]) => (
              <tr key={key}>
                <td style={{ padding: '0.5rem 1rem', fontWeight: 'bold', border: '1px solid #ccc' }}>
                  {key}
                </td>
                <td style={{ padding: '0.5rem 1rem', border: '1px solid #ccc' }}>
                  {String(value)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {!health && !error && <p>Loading...</p>}
    </div>
  );
}

export default App
