import { NavLink, Outlet, useNavigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <nav style={{
        background: 'var(--navy)',
        borderBottom: '3px solid var(--orange)',
        padding: '0 1.5rem',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        height: '56px',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '2rem' }}>
          <span style={{
            color: 'var(--orange)',
            fontWeight: 700,
            fontSize: '1.25rem',
            letterSpacing: '-0.5px',
          }}>
            RS Madness
          </span>
          <div style={{ display: 'flex', gap: '1rem' }}>
            <NavLink to="/ranking" style={({ isActive }) => ({
              color: isActive ? 'var(--orange)' : '#94a3b8',
              textDecoration: 'none',
              fontSize: '0.9rem',
              fontWeight: isActive ? 600 : 400,
              padding: '0.25rem 0',
              borderBottom: isActive ? '2px solid var(--orange)' : '2px solid transparent',
            })}>
              Ranking
            </NavLink>
            <NavLink to="/leaderboard" style={({ isActive }) => ({
              color: isActive ? 'var(--orange)' : '#94a3b8',
              textDecoration: 'none',
              fontSize: '0.9rem',
              fontWeight: isActive ? 600 : 400,
              padding: '0.25rem 0',
              borderBottom: isActive ? '2px solid var(--orange)' : '2px solid transparent',
            })}>
              Leaderboard
            </NavLink>
          </div>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <span style={{ color: '#94a3b8', fontSize: '0.85rem' }}>
            {user?.displayName}
          </span>
          <button
            onClick={handleLogout}
            style={{
              background: 'transparent',
              border: '1px solid #94a3b8',
              color: '#94a3b8',
              padding: '0.3rem 0.75rem',
              borderRadius: '4px',
              cursor: 'pointer',
              fontSize: '0.8rem',
            }}
          >
            Logout
          </button>
        </div>
      </nav>

      <main style={{
        flex: 1,
        padding: '1.5rem',
        maxWidth: '1126px',
        width: '100%',
        margin: '0 auto',
        boxSizing: 'border-box',
      }}>
        <Outlet />
      </main>
    </div>
  );
}