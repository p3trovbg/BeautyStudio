import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
    token: string | null;
    role: 'Owner' | 'Customer' | null;
    userId: string | null;
    isAuthenticated: boolean;
    login: (token: string, role: 'Owner' | 'Customer', userId: string) => void;
    logout: () => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set: (partial: Partial<AuthState>) => void) => ({
            token: null,
            role: null,
            userId: null,
            isAuthenticated: false,
            login: (token: string, role: 'Owner' | 'Customer', userId: string) =>
                set({ token, role, userId, isAuthenticated: true }),
            logout: () =>
                set({ token: null, role: null, userId: null, isAuthenticated: false }),
        }),
        { name: 'auth-storage' }
    )
);
