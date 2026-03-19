import { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import apiClient from '../api/client';

// types
interface User {
    displayName: string;
    email: string;
}

interface AuthContextType {
    user: User | null;
    loading: boolean;
    login: (email: string, password: string) => Promise<void>;
    register: (email: string, password: string, displayName: string) => Promise<void>;
    logout: () => void;
}

// context
const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('token');
        
        // no token
        if (!token) {
            setLoading(false);
            return;
        }

        // otherwise validate token
        apiClient.get<User>('/auth/me')
            .then(res => setUser(res.data))
            .catch(() => localStorage.removeItem('token'))
            .finally(() => setLoading(false));
    }, []);

    // login
    const login = async (email: string, password: string) => {
        const res = await apiClient.post<{ token: string; displayName: string; email: string }>('/auth/login', { email, password });
        localStorage.setItem('token', res.data.token);
        setUser({ displayName: res.data.displayName, email: res.data.email });
    };

    // register
    const register = async (email: string, password: string, displayName: string) => {
        const res = await apiClient.post<{ token: string; displayName: string; email: string }>('/auth/register', { email, password, displayName });
        localStorage.setItem('token', res.data.token);
        setUser({ displayName: res.data.displayName, email: res.data.email });
    };

    // logout
    const logout = () => {
        localStorage.removeItem('token');
        setUser(null);
    };

    // provide state to child components

    return (
        <AuthContext.Provider value={{ user, loading, login, register, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

// hook to be called
export function useAuth(): AuthContextType {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}