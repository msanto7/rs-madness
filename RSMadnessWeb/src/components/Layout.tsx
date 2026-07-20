import { useEffect, useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';
import apiClient from '../api/client';
import './Layout.css';

const navItems = [
  { to: '/ranking', label: 'Ranking' },
  { to: '/leaderboard', label: 'Leaderboard' },
  { to: '/blank-bracket', label: 'In Honor of Those Before Us' },
];

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [hideRanking, setHideRanking] = useState(false);

  // hide the Ranking tab once the deadline has passed for anyone who never submitted --
  // RankingPage itself still enforces this on direct navigation, this is just nav visibility
  useEffect(() => {
    let cancelled = false;

    apiClient
      .get<{ isPassed: boolean }>('/bracketentry/submission-deadline')
      .then(async (deadlineRes) => {
        if (cancelled || !deadlineRes.data.isPassed) return;

        // only need to know submission status once the deadline has actually passed
        const entryRes = await apiClient
          .get<{ submittedAt: string | null }>('/bracketentry/me')
          .catch(() => null);

        if (cancelled) return;
        const isSubmitted = entryRes?.data.submittedAt != null;
        setHideRanking(!isSubmitted);
      })
      .catch(() => {
        // fail open -- keep the tab visible if the status check itself fails
      });

    return () => {
      cancelled = true;
    };
  }, []);

  const visibleNavItems = navItems.filter((item) => item.to !== '/ranking' || !hideRanking);

  const handleLogout = async () => {
    await logout();
    setIsMobileMenuOpen(false);
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
            {visibleNavItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                onClick={() => setIsMobileMenuOpen(false)}
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
            {visibleNavItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                onClick={() => setIsMobileMenuOpen(false)}
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
