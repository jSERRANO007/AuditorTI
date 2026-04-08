import { type Configuration, type PopupRequest, LogLevel } from '@azure/msal-browser';

export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_CLIENT_ID || '',
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AZURE_TENANT_ID || ''}`,
    redirectUri: import.meta.env.VITE_REDIRECT_URI || window.location.origin,
    postLogoutRedirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'sessionStorage',
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        if (level === LogLevel.Error) console.error('[MSAL]', message);
      },
    },
  },
};

export const loginRequest: PopupRequest = {
  scopes: [
    `api://auditorpro-ti/Simulaciones.Read`,
    `api://auditorpro-ti/Simulaciones.Write`,
    `api://auditorpro-ti/Hallazgos.Read`,
  ],
};

export const apiScopes = [`${import.meta.env.VITE_API_SCOPE || 'api://auditorpro-ti/Simulaciones.Read'}`];
