import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';
import './AuthPages.css';

export default function RegisterPage() {
  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState<string[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const navigate = useNavigate();
  const { register } = useAuth();

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setErrors([]);
    setIsSubmitting(true);

    if (password !== confirmPassword) {
      setErrors(['Passwords do not match']);
      setIsSubmitting(false);
      return;
    }

    try {
      await register(email, password, displayName);
      navigate('/ranking');
    } catch (err: any) {
      const data = err.response?.data;
      if (Array.isArray(data)) {
        setErrors(data.map((e: any) => e.description));
      } else if (data?.errors) {
        setErrors(Object.values(data.errors).flat() as string[]);
      } else {
        setErrors([data?.error ?? data ?? 'Registration failed']);
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
          <h1 className="auth-card__title">Create Account</h1>
          <p className="auth-card__subtitle">Set up your profile to submit rankings and compete in the pool.</p>
        </div>

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="auth-form__group">
            <label className="auth-form__label" htmlFor="register-display-name">Display Name</label>
            <input
              id="register-display-name"
              type="text"
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
              required
              className="auth-form__input"
            />
          </div>

          <div className="auth-form__group">
            <label className="auth-form__label" htmlFor="register-email">Email</label>
            <input
              id="register-email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="auth-form__input"
            />
          </div>

          <div className="auth-form__group">
            <label className="auth-form__label" htmlFor="register-password">Password</label>
            <input
              id="register-password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              minLength={12}
              className="auth-form__input"
            />
          </div>

          <div className="auth-form__group">
            <label className="auth-form__label" htmlFor="register-confirm-password">Confirm Password</label>
            <input
              id="register-confirm-password"
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
              className="auth-form__input"
            />
          </div>

          {errors.length > 0 && (
            <ul className="auth-card__error-list">
              {errors.map((msg) => (
                <li key={msg}>{msg}</li>
              ))}
            </ul>
          )}

          <button type="submit" className="auth-form__button" disabled={isSubmitting}>
            {isSubmitting ? 'Creating Account...' : 'Create Account'}
          </button>
        </form>

        <p className="auth-card__footer">
          Already have an account? <Link to="/login">Log In</Link>
        </p>
      </section>
    </div>
  );
}
