import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';
import './AuthPages.css';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const navigate = useNavigate();

  const { login } = useAuth();

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);

    try {
      await login(email, password);
      navigate('/ranking');
    } catch (err: any) {
      const data = err.response?.data;

      if (data?.errors) {
        const messages = Object.values(data.errors).flat().join(' ');
        setError(messages);
      } else {
        setError(data?.error ?? data ?? 'Login failed');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="auth-page">
      <section className="auth-card">
        <div className="auth-card__header">
          <span className="auth-card__eyebrow">RS Madness</span>
          <h1 className="auth-card__title">Welcome Back</h1>
          <p className="auth-card__subtitle">Log in to update your rankings and check the leaderboard.</p>
        </div>

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="auth-form__group">
            <label className="auth-form__label" htmlFor="login-email">Email</label>
            <input
              id="login-email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="auth-form__input"
            />
          </div>

          <div className="auth-form__group">
            <label className="auth-form__label" htmlFor="login-password">Password</label>
            <input
              id="login-password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="auth-form__input"
            />
          </div>

          {error && <p className="auth-card__error">{error}</p>}

          <button type="submit" className="auth-form__button" disabled={isSubmitting}>
            {isSubmitting ? 'Logging In...' : 'Log In'}
          </button>
        </form>

        <p className="auth-card__footer">
          Don&apos;t have an account? <Link to="/register">Register</Link>
        </p>
      </section>
    </div>
  );
}
