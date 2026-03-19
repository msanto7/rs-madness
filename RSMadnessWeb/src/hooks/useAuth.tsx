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
    register: (email: string, password: string) => Promise<void>;
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

    // register

    // logout

    // provide state to child components



    // hook to be called