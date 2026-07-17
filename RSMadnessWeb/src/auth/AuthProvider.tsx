import { useCallback, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import apiClient from '../api/client';
import { onAuthExpired } from './sessionEvents';
import { AuthContext } from './AuthContext';
import type { User } from './AuthContext';

const idleLogoutMs = 60 * 60 * 1000;
const activityEvents = ['pointerdown', 'keydown', 'scroll', 'touchstart'] as const;

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        apiClient.get<User>('/auth/me')
            .then(res => setUser(res.data))
            .catch(() => setUser(null))
            .finally(() => setLoading(false));
    }, []);

    useEffect(() => {
        return onAuthExpired(() => {
            setUser(null);
        });
    }, []);

    const login = async (email: string, password: string) => {
        const res = await apiClient.post<User>('/auth/login', { email, password });
        setUser({ displayName: res.data.displayName, email: res.data.email });
    };

    const register = async (email: string, password: string, displayName: string) => {
        const res = await apiClient.post<User>('/auth/register', { email, password, displayName });
        setUser({ displayName: res.data.displayName, email: res.data.email });
    };

    const logout = useCallback(async () => {
        try {
            await apiClient.post('/auth/logout');
        } catch {
            // best-effort: clear local session state below even if the request fails
        }
        setUser(null);
    }, []);

    useEffect(() => {
        if (!user) return;

        let idleTimer: ReturnType<typeof window.setTimeout> | undefined;

        const resetIdleTimer = () => {
            if (idleTimer !== undefined) window.clearTimeout(idleTimer);
            idleTimer = window.setTimeout(() => {
                void logout();
            }, idleLogoutMs);
        };

        resetIdleTimer();
        activityEvents.forEach((eventName) => window.addEventListener(eventName, resetIdleTimer, { passive: true }));

        return () => {
            if (idleTimer !== undefined) window.clearTimeout(idleTimer);
            activityEvents.forEach((eventName) => window.removeEventListener(eventName, resetIdleTimer));
        };
    }, [logout, user]);

    return (
        <AuthContext.Provider value={{ user, loading, login, register, logout }}>
            {children}
        </AuthContext.Provider>
    );
}
