import { useEffect, useState } from 'react';
import { getHallazgos, actualizarPlanAccion, type HallazgoDto } from '../api/hallazgos';
import { Semaforo } from '../components/ui/Semaforo';
import { toast } from 'sonner';

const criticidadColor: Record<string, string> = {
  CRITICA: 'bg-red-100 text-red-800',
  MEDIA: 'bg-yellow-100 text-yellow-800',
  BAJA: 'bg-blue-100 text-blue-800',
};

const semaforoMap: Record<string, 'VERDE' | 'AMARILLO' | 'ROJO' | 'NO_EVALUADO'> = {
  ABIERTO: 'ROJO',
  EN_PROCESO: 'AMARILLO',
  RESUELTO: 'VERDE',
  CERRADO: 'VERDE',
  ACEPTADO: 'AMARILLO',
};

export function Hallazgos() {
  const [items, setItems] = useState<HallazgoDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selected, setSelected] = useState<HallazgoDto | null>(null);
  const [planAccion, setPlanAccion] = useState('');

  useEffect(() => {
    getHallazgos()
      .then((r) => setItems(r.items))
      .catch(() => toast.error('Error al cargar hallazgos'))
      .finally(() => setLoading(false));
  }, []);

  const guardarPlan = async () => {
    if (!selected) return;
    try {
      await actualizarPlanAccion(selected.id, { planAccion });
      toast.success('Plan de acción guardado');
      setSelected(null);
    } catch {
      toast.error('Error al guardar plan');
    }
  };

  return (
    <div className="p-6 space-y-4">
      <h2 className="text-2xl font-bold text-gray-900">Hallazgos</h2>

      <div className="bg-white rounded-xl shadow-sm border overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-gray-400">Cargando...</div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600">Estado</th>
                <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600">Título</th>
                <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600">Criticidad</th>
                <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600">Norma</th>
                <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600">Responsable</th>
                <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600">Compromiso</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((h) => (
                <tr key={h.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3">
                    <Semaforo valor={semaforoMap[h.estado] ?? 'NO_EVALUADO'} showLabel />
                  </td>
                  <td className="px-4 py-3 font-medium text-gray-800 max-w-xs truncate">{h.titulo}</td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex px-2 py-0.5 rounded-full text-xs font-medium ${criticidadColor[h.criticidad] ?? 'bg-gray-100'}`}>
                      {h.criticidad}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-gray-500 text-xs">{h.normaAfectada ?? '—'}</td>
                  <td className="px-4 py-3 text-gray-500 text-xs">{h.responsableEmail ?? '—'}</td>
                  <td className="px-4 py-3 text-gray-500 text-xs">{h.fechaCompromiso ?? '—'}</td>
                  <td className="px-4 py-3">
                    <button
                      onClick={() => { setSelected(h); setPlanAccion(h.planAccion ?? ''); }}
                      className="text-blue-600 hover:text-blue-800 text-xs"
                    >
                      Plan de acción
                    </button>
                  </td>
                </tr>
              ))}
              {items.length === 0 && (
                <tr><td colSpan={7} className="px-4 py-8 text-center text-gray-400">Sin hallazgos</td></tr>
              )}
            </tbody>
          </table>
        )}
      </div>

      {/* Plan de acción modal */}
      {selected && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-lg shadow-xl">
            <h3 className="font-semibold text-gray-800 mb-2">Plan de Acción</h3>
            <p className="text-sm text-gray-600 mb-3">{selected.titulo}</p>
            <textarea
              value={planAccion}
              onChange={(e) => setPlanAccion(e.target.value)}
              rows={5}
              className="w-full border rounded-lg px-3 py-2 text-sm"
              placeholder="Describe el plan de acción correctiva..."
            />
            <div className="flex gap-2 mt-3">
              <button onClick={guardarPlan} className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm hover:bg-blue-700">
                Guardar
              </button>
              <button onClick={() => setSelected(null)} className="border px-4 py-2 rounded-lg text-sm text-gray-600 hover:bg-gray-50">
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
