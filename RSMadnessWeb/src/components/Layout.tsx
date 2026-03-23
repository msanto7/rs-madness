import { useEffect, useState } from 'react';
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';
import './Layout.css';

const navItems = [
  { to: '/ranking', label: 'Ranking' },
  { to: '/leaderboard', label: 'Leaderboard' },
  { to: '/blank-bracket', label: 'In Honor of Those Before Us' },
];

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    setIsMobileMenuOpen(false);
  }, [location.pathname]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="app-shell">
      <header className="top-nav">
        <div className="top-nav__bar">
          <div className="top-nav__brand-wrap">
            <span className="top-nav__brand">RS Madness</span>
            <span className="top-nav__badge">2026</span>
          </div>

          <nav className="top-nav__links top-nav__links--desktop">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) => `top-nav__link ${isActive ? 'top-nav__link--active' : ''}`}
              >
                {item.label}
              </NavLink>
            ))}
          </nav>

          <div className="top-nav__actions top-nav__actions--desktop">
            <span className="top-nav__user">{user?.displayName}</span>
            <button onClick={handleLogout} className="top-nav__logout-btn">
              Logout
            </button>
          </div>

          <button
            type="button"
            className="top-nav__menu-btn"
            aria-label={isMobileMenuOpen ? 'Close navigation menu' : 'Open navigation menu'}
            aria-expanded={isMobileMenuOpen}
            onClick={() => setIsMobileMenuOpen((prev) => !prev)}
          >
            {isMobileMenuOpen ? 'Close' : 'Menu'}
          </button>
        </div>

        <div className={`top-nav__mobile-panel ${isMobileMenuOpen ? 'top-nav__mobile-panel--open' : ''}`}>
          <nav className="top-nav__links top-nav__links--mobile">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) => `top-nav__link ${isActive ? 'top-nav__link--active' : ''}`}
              >
                {item.label}
              </NavLink>
            ))}
          </nav>
          <div className="top-nav__actions top-nav__actions--mobile">
            <span className="top-nav__user">{user?.displayName}</span>
            <button onClick={handleLogout} className="top-nav__logout-btn">
              Logout
            </button>
          </div>
        </div>
      </header>

      <main className="app-shell__main">
        <Outlet />
      </main>
    </div>
  );
}
