import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { msalInstance } from '../store/authStore';
import { apiScopes } from '../config/auth';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000/api',
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' },
});

// Attach Bearer token on every request
api.interceptors.request.use(async (config: InternalAxiosRequestConfig) => {
  const accounts = msalInstance.getAllAccounts();
  if (accounts.length > 0) {
    try {
      const result = await msalInstance.acquireTokenSilent({
        scopes: apiScopes,
        account: accounts[0],
      });
      config.headers.Authorization = `Bearer ${result.accessToken}`;
    } catch {
      await msalInstance.acquireTokenRedirect({ scopes: apiScopes });
    }
  }
  return config;
});

// Response error handling
api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      msalInstance.acquireTokenRedirect({ scopes: apiScopes });
    }
    return Promise.reject(error);
  }
);

export default api;
