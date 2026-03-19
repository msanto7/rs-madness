import { useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';

export default function LoginPage() {
    // form state
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const navigate = useNavigate();

    const { login } = useAuth();

    const handleSubmit = async (e: { preventDefault: () => void }) => {
        e.preventDefault();
        setError('');

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
        }
    };

    return (
    <div style={{ maxWidth: 400, margin: '4rem auto', padding: '2rem' }}>
      <h1>Log In</h1>

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '1rem' }}>
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            style={{ display: 'block', width: '100%', padding: '0.5rem' }}
          />
        </div>

        <div style={{ marginBottom: '1rem' }}>
          <label>Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            style={{ display: 'block', width: '100%', padding: '0.5rem' }}
          />
        </div>

        {error && <p style={{ color: 'red' }}>{error}</p>}

        <button type="submit" style={{ padding: '0.5rem 1.5rem' }}>
          Log In
        </button>
      </form>

      <p style={{ marginTop: '1rem' }}>
        Don't have an account? <Link to="/register">Register</Link>
      </p>
    </div>
  );
}