import { useState } from "react";
import { Link, useNavigate } from "react-router";
import { useAuth } from "../hooks/useAuth";

export default function RegisterPage() {
    const [displayName, setDisplayName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [errors, setErrors] = useState<string[]>([]);

    const navigate = useNavigate();
    const { register } = useAuth();

    const handleSubmit = async (e: { preventDefault: () => void }) => {
        e.preventDefault();
        setErrors([]);

        if (password !== confirmPassword) {
            setErrors(["Passwords do not match"]);
            return;
        }

        try {
            await register(email, password, displayName);
            navigate("/ranking");
        } catch (err: any) {
            const data = err.response?.data;
            if (Array.isArray(data)) {
                setErrors(data.map((e: any) => e.description));
            } else if (data?.errors) {
                setErrors(Object.values(data.errors).flat() as string[]);
            } else {
                setErrors([data?.error ?? data ?? "Registration failed"]);
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

        {errors.length > 0 && (
            <ul style={{ color: 'red', paddingLeft: '1.2rem' }}>
                {errors.map((msg, i) => (
                    <li key={i}>{msg}</li>
                ))}
            </ul>
        )}

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