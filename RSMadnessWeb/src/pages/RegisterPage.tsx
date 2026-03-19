import { useState } from "react";
import { Link, useNavigate } from "react-router";
import { useAuth } from "../hooks/useAuth";

export default function RegisterPage() {
    const [displayName, setDisplayName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");

    const navigate = useNavigate();
    const { register } = useAuth();

    const handleSubmit = async (e: { preventDefault: () => void }) => {
        e.preventDefault();
        setError("");

        if (password !== confirmPassword) {
            setError("Passwords do not match");
            return;
        }

        try {
            await register(email, password, displayName);
            navigate("/ranking");
        } catch (err: any) {
            const data = err.response?.data;
            if (data?.errors) {
                const messages = Array.isArray(data.errors) ? data.errors.map((e: any) => e.msg).join(", ") : String(data.errors);
                setError(messages);
            } else {
                setError(data?.error ?? data ?? "Registration failed");
            }
        }
    };

    return (
        <div style={{ maxWidth: 400, margin: '4rem auto', padding: '2rem' }}>
      <h1>Register</h1>

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '1rem' }}>
          <label>Display Name</label>
          <input
            type="text"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            required
            style={{ display: 'block', width: '100%', padding: '0.5rem' }}
          />
        </div>

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
            minLength={12}
            style={{ display: 'block', width: '100%', padding: '0.5rem' }}
          />
        </div>

        <div style={{ marginBottom: '1rem' }}>
          <label>Confirm Password</label>
          <input
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
            style={{ display: 'block', width: '100%', padding: '0.5rem' }}
          />
        </div>

        {error && <p style={{ color: 'red' }}>{error}</p>}

        <button type="submit" style={{ padding: '0.5rem 1.5rem' }}>
          Create Account
        </button>
      </form>

      <p style={{ marginTop: '1rem' }}>
        Already have an account? <Link to="/login">Log in</Link>
      </p>
    </div>
  );
}