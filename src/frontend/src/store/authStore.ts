import { PublicClientApplication, type AccountInfo } from '@azure/msal-browser';
import { create } from 'zustand';
import { msalConfig } from '../config/auth';

export const msalInstance = new PublicClientApplication(msalConfig);

interface AuthState {
  user: AccountInfo | null;
  isAuthenticated: boolean;
  roles: string[];
  setUser: (user: AccountInfo | null) => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: false,
  roles: [],
  setUser: (user) =>
    set({
      user,
      isAuthenticated: !!user,
      roles: (user?.idTokenClaims as Record<string, unknown>)?.roles as string[] ?? [],
    }),
}));

export const hasRole = (roles: string[], required: string[]) =>
  required.some((r) => roles.includes(r));
