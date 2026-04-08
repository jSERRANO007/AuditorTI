import { NavLink, Outlet } from 'react-router-dom';
import { useMsal } from '@azure/msal-react';
import {
  LayoutDashboard, PlayCircle, AlertTriangle, FileCheck,
  Brain, Plug, FileText, BookOpen, LogOut
} from 'lucide-react';

const navItems = [
  { to: '/dashboard', label: 'Dashboard', icon: <LayoutDashboard size={18} /> },
  { to: '/simulaciones', label: 'Simulaciones', icon: <PlayCircle size={18} /> },
  { to: '/hallazgos', label: 'Hallazgos', icon: <AlertTriangle size={18} /> },
  { to: '/evidencias', label: 'Evidencias', icon: <FileCheck size={18} /> },
  { to: '/ia', label: 'Agente IA', icon: <Brain size={18} /> },
  { to: '/conectores', label: 'Conectores', icon: <Plug size={18} /> },
  { to: '/politicas', label: 'Políticas', icon: <FileText size={18} /> },
  { to: '/bitacora', label: 'Bitácora', icon: <BookOpen size={18} /> },
];

export function AppLayout() {
  const { instance, accounts } = useMsal();
  const user = accounts[0];

  return (
    <div className="flex h-screen bg-gray-100">
      {/* Sidebar */}
      <aside className="w-56 bg-gray-900 text-white flex flex-col shadow-xl">
        <div className="px-4 py-5 border-b border-gray-700">
          <h1 className="text-lg font-bold text-blue-400">AuditorPRO</h1>
          <p className="text-xs text-gray-400">Auditoría Preventiva TI</p>
        </div>

        <nav className="flex-1 px-2 py-4 space-y-1">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm transition-colors ${
                  isActive
                    ? 'bg-blue-600 text-white'
                    : 'text-gray-300 hover:bg-gray-800 hover:text-white'
                }`
              }
            >
              {item.icon}
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="px-4 py-4 border-t border-gray-700">
          <p className="text-xs text-gray-400 truncate">{user?.username}</p>
          <button
            onClick={() => instance.logoutRedirect()}
            className="mt-2 flex items-center gap-2 text-xs text-gray-400 hover:text-white transition-colors"
          >
            <LogOut size={14} />
            Cerrar sesión
          </button>
        </div>
      </aside>

      {/* Main content */}
      <main className="flex-1 overflow-auto">
        <Outlet />
      </main>
    </div>
  );
}
