import { useEffect, useState } from 'react';
import apiClient from '../api/client';

interface LeaderboardEntry {
  position: number;
  userDisplayName: string;
  currentPoints: number;
  potentialPoints: number;
  submittedAt: string;
}

export default function LeaderboardPage() {
  const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    apiClient.get<LeaderboardEntry[]>('/leaderboard')
      .then(res => setEntries(res.data))
      .catch(() => setError('Failed to load leaderboard.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Loading...</p>;
  if (error) return <p style={{ color: '#ef4444' }}>{error}</p>;

  return (
    <div>
      <h1 style={{ marginBottom: '1rem' }}>Leaderboard</h1>

      {entries.length === 0 ? (
        <p style={{ color: '#94a3b8' }}>No submitted entries yet.</p>
      ) : (
        <table style={{
          width: '100%',
          borderCollapse: 'collapse',
          fontSize: '0.95rem',
        }}>
          <thead>
            <tr style={{
              borderBottom: '2px solid var(--border)',
              fontSize: '0.75rem',
              color: '#64748b',
              textTransform: 'uppercase',
              letterSpacing: '0.5px',
            }}>
              <th style={thStyle}>#</th>
              <th style={{ ...thStyle, textAlign: 'left' }}>Player</th>
              <th style={thStyle}>Points</th>
              <th style={thStyle}>Potential</th>
              <th style={thStyle}>Submitted</th>
            </tr>
          </thead>
          <tbody>
            {entries.map((entry) => (
              <tr key={entry.position} style={{
                borderBottom: '1px solid var(--border)',
              }}>
                <td style={{
                  ...tdStyle,
                  fontWeight: 700,
                  color: entry.position <= 3 ? 'var(--orange)' : 'var(--text)',
                }}>
                  {entry.position}
                </td>
                <td style={{ ...tdStyle, textAlign: 'left', color: 'var(--text-h)' }}>
                  {entry.userDisplayName}
                </td>
                <td style={{ ...tdStyle, fontWeight: 600 }}>
                  {entry.currentPoints}
                </td>
                <td style={tdStyle}>
                  {entry.potentialPoints}
                </td>
                <td style={{ ...tdStyle, fontSize: '0.8rem', color: '#64748b' }}>
                  {new Date(entry.submittedAt).toLocaleDateString()}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding: '0.6rem 1rem',
  textAlign: 'center',
  fontWeight: 600,
};

const tdStyle: React.CSSProperties = {
  padding: '0.75rem 1rem',
  textAlign: 'center',
};