import { useEffect, useState } from 'react';
import { getDashboard, type DashboardDto } from '../api/dashboard';
import { KpiTile } from '../components/ui/KpiTile';
import { ScoreMadurez } from '../components/ui/ScoreMadurez';
import { Semaforo } from '../components/ui/Semaforo';
import { AlertTriangle, CheckCircle, Activity, PlayCircle } from 'lucide-react';
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, LineChart, Line, CartesianGrid } from 'recharts';
import { toast } from 'sonner';

export function Dashboard() {
  const [data, setData] = useState<DashboardDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getDashboard()
      .then(setData)
      .catch(() => toast.error('Error al cargar el dashboard'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-full">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" />
      </div>
    );
  }

  if (!data) return null;

  return (
    <div className="p-6 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Dashboard Ejecutivo</h2>
          <p className="text-sm text-gray-500 mt-1">Estado de madurez y cumplimiento en tiempo real</p>
        </div>
      </div>

      {/* KPI Row */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <KpiTile
          label="Hallazgos Abiertos"
          value={data.totalHallazgosAbiertos}
          sublabel={`${data.hallazgosCriticos} críticos`}
          color={data.hallazgosCriticos > 0 ? 'red' : 'green'}
          icon={<AlertTriangle size={24} />}
        />
        <KpiTile
          label="Cumplimiento"
          value={`${data.porcentajeCumplimiento.toFixed(1)}%`}
          color={data.porcentajeCumplimiento >= 80 ? 'green' : data.porcentajeCumplimiento >= 60 ? 'yellow' : 'red'}
          icon={<CheckCircle size={24} />}
        />
        <KpiTile
          label="Simulaciones (30d)"
          value={data.simulacionesUltimos30Dias}
          color="blue"
          icon={<PlayCircle size={24} />}
        />
        <KpiTile
          label="Controles Rojo"
          value={data.semaforoResumen.rojo}
          sublabel={`de ${data.semaforoResumen.total} evaluados`}
          color={data.semaforoResumen.rojo > 0 ? 'red' : 'green'}
          icon={<Activity size={24} />}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Score de madurez */}
        <div className="bg-white rounded-xl shadow-sm border p-6 flex flex-col items-center justify-center">
          <h3 className="text-sm font-semibold text-gray-600 mb-4">Score de Madurez Global</h3>
          <ScoreMadurez score={data.scoreMadurezGlobal} />
          <div className="mt-4 grid grid-cols-3 gap-3 w-full">
            <div className="text-center">
              <div className="text-xl font-bold text-green-600">{data.semaforoResumen.verde}</div>
              <div className="text-xs text-gray-500">Verde</div>
            </div>
            <div className="text-center">
              <div className="text-xl font-bold text-yellow-500">{data.semaforoResumen.amarillo}</div>
              <div className="text-xs text-gray-500">Amarillo</div>
            </div>
            <div className="text-center">
              <div className="text-xl font-bold text-red-600">{data.semaforoResumen.rojo}</div>
              <div className="text-xs text-gray-500">Rojo</div>
            </div>
          </div>
        </div>

        {/* Tendencia */}
        <div className="bg-white rounded-xl shadow-sm border p-6 lg:col-span-2">
          <h3 className="text-sm font-semibold text-gray-600 mb-4">Tendencia de Score — Últimos 6 meses</h3>
          <ResponsiveContainer width="100%" height={180}>
            <LineChart data={data.tendenciaUltimos6Meses}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
              <XAxis dataKey="periodo" tick={{ fontSize: 11 }} />
              <YAxis domain={[0, 10]} tick={{ fontSize: 11 }} />
              <Tooltip />
              <Line type="monotone" dataKey="score" stroke="#3b82f6" strokeWidth={2} dot={{ r: 4 }} name="Score" />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Hallazgos críticos */}
        <div className="bg-white rounded-xl shadow-sm border p-6">
          <h3 className="text-sm font-semibold text-gray-600 mb-4">Hallazgos Críticos Pendientes</h3>
          <div className="space-y-3">
            {data.topHallazgosCriticos.length === 0 && (
              <p className="text-sm text-gray-400 text-center py-4">Sin hallazgos críticos</p>
            )}
            {data.topHallazgosCriticos.map((h) => (
              <div key={h.id} className="flex items-start justify-between gap-2 py-2 border-b last:border-0">
                <div className="flex items-start gap-2">
                  <Semaforo valor="ROJO" size="sm" />
                  <div>
                    <p className="text-sm font-medium text-gray-800">{h.titulo}</p>
                    <p className="text-xs text-gray-500">{h.estado}</p>
                  </div>
                </div>
                {h.fechaCompromiso && (
                  <span className="text-xs text-gray-400 whitespace-nowrap">{h.fechaCompromiso}</span>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Puntaje por dominio */}
        <div className="bg-white rounded-xl shadow-sm border p-6">
          <h3 className="text-sm font-semibold text-gray-600 mb-4">Score por Dominio</h3>
          <ResponsiveContainer width="100%" height={200}>
            <BarChart data={data.puntajePorDominio} layout="vertical">
              <XAxis type="number" domain={[0, 10]} tick={{ fontSize: 10 }} />
              <YAxis dataKey="dominio" type="category" tick={{ fontSize: 10 }} width={100} />
              <Tooltip />
              <Bar dataKey="score" fill="#3b82f6" radius={[0, 4, 4, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}
