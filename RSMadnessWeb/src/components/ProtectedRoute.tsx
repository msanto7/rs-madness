import { Navigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';

// for pages that require auth
export default function ProtectedRoute({ children }: { children: React.ReactNode })
{
    const { user, loading } = useAuth();

    // loading state for checking token
    if (loading) return <div>Loading...</div>;

    // any protected page with no logged in user - redirect to login
    if (!user) return <Navigate to="/login" replace />;

    // otherwise show page
    return <>{children}</>;
}