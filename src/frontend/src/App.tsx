import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { MsalProvider, AuthenticatedTemplate, UnauthenticatedTemplate } from '@azure/msal-react';
import { Toaster } from 'sonner';
import { msalInstance } from './store/authStore';
import { AppLayout } from './components/layout/AppLayout';
import { Dashboard } from './pages/Dashboard';
import { Simulaciones } from './pages/Simulaciones';
import { Hallazgos } from './pages/Hallazgos';
import { AgenteIA } from './pages/AgenteIA';
import { PlaceholderPage } from './pages/PlaceholderPage';
import { loginRequest } from './config/auth';

function LoginScreen() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-blue-900 flex items-center justify-center">
      <div className="bg-white rounded-2xl shadow-2xl p-8 max-w-sm w-full text-center">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900">AuditorPRO TI</h1>
          <p className="text-sm text-gray-500 mt-1">Plataforma de Auditoría Preventiva Inteligente</p>
        </div>
        <p className="text-sm text-gray-600 mb-6">
          Autenticación corporativa con Microsoft Entra ID
        </p>
        <button
          onClick={() => msalInstance.loginRedirect(loginRequest)}
          className="w-full bg-blue-600 text-white py-2.5 rounded-xl font-medium hover:bg-blue-700 transition"
        >
          Iniciar sesión con Microsoft
        </button>
        <p className="text-xs text-gray-400 mt-4">ILG Logistics — Uso interno confidencial</p>
      </div>
    </div>
  );
}

export default function App() {
  return (
    <MsalProvider instance={msalInstance}>
      <Toaster position="top-right" richColors />
      <AuthenticatedTemplate>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<AppLayout />}>
              <Route index element={<Navigate to="/dashboard" replace />} />
              <Route path="dashboard" element={<Dashboard />} />
              <Route path="simulaciones" element={<Simulaciones />} />
              <Route path="hallazgos" element={<Hallazgos />} />
              <Route path="evidencias" element={<PlaceholderPage title="Evidencias" description="Gestión de evidencias documentales" />} />
              <Route path="ia" element={<AgenteIA />} />
              <Route path="conectores" element={<PlaceholderPage title="Conectores" description="SOA Manager — Integración con sistemas externos" />} />
              <Route path="politicas" element={<PlaceholderPage title="Políticas y Procedimientos" description="Catálogo y control de vigencia" />} />
              <Route path="bitacora" element={<PlaceholderPage title="Bitácora de Auditoría" description="Registro inmutable de eventos del sistema" />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </AuthenticatedTemplate>
      <UnauthenticatedTemplate>
        <LoginScreen />
      </UnauthenticatedTemplate>
    </MsalProvider>
  );
}
