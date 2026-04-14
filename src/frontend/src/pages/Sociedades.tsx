import { useState, useEffect, useRef } from 'react';
import api from '../api/client';
import { toast } from 'sonner';
import {
  Plus, Pencil, Power, Upload, Download, RefreshCw,
  CheckCircle2, XCircle, X, Search, Globe
} from 'lucide-react';

interface Sociedad {
  id: number;
  codigo: string;
  nombre: string;
  pais?: string;
  activa: boolean;
}

const PAISES = ['Costa Rica','Panamá','Guatemala','Honduras','El Salvador','Nicaragua','República Dominicana','Otro'];

function ModalSociedad({ sociedad, onClose, onSaved }: {
  sociedad?: Sociedad; onClose: () => void; onSaved: (s: Sociedad) => void;
}) {
  const [codigo, setCodigo] = useState(sociedad?.codigo ?? '');
  const [nombre, setNombre] = useState(sociedad?.nombre ?? '');
  const [pais,   setPais]   = useState(sociedad?.pais ?? '');
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      const res = sociedad
        ? await api.put(`/sociedades/${sociedad.id}`, { nombre, pais })
        : await api.post('/sociedades', { codigo, nombre, pais });
      onSaved(res.data);
      toast.success(sociedad ? 'Sociedad actualizada' : 'Sociedad creada');
      onClose();
    } catch (err: any) {
      toast.error(err?.response?.data?.error ?? 'Error al guardar');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-md">
        <div className="flex items-center justify-between px-6 py-4 border-b">
          <h2 className="font-semibold text-gray-800">{sociedad ? 'Editar Sociedad' : 'Nueva Sociedad'}</h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600"><X size={18} /></button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          {!sociedad && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Código SAP *</label>
              <input value={codigo} onChange={e => setCodigo(e.target.value.toUpperCase())}
                maxLength={10} required placeholder="Ej: CR15"
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm font-mono focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
          )}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Nombre *</label>
            <input value={nombre} onChange={e => setNombre(e.target.value)}
              required placeholder="Nombre completo de la sociedad"
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">País</label>
            <select value={pais} onChange={e => setPais(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">— Seleccionar —</option>
              {PAISES.map(p => <option key={p} value={p}>{p}</option>)}
            </select>
          </div>
          <div className="flex gap-3 pt-2">
            <button type="submit" disabled={saving}
              className="flex-1 bg-blue-600 text-white py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition flex items-center justify-center gap-2">
              {saving && <RefreshCw size={14} className="animate-spin" />}
              {sociedad ? 'Guardar cambios' : 'Crear sociedad'}
            </button>
            <button type="button" onClick={onClose} className="px-4 py-2 text-sm text-gray-600 hover:text-gray-800">Cancelar</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export function Sociedades() {
  const [sociedades, setSociedades] = useState<Sociedad[]>([]);
  const [loading,    setLoading]    = useState(true);
  const [filtro,     setFiltro]     = useState('');
  const [soloActivas, setSoloActivas] = useState(false);
  const [modalOpen,  setModalOpen]  = useState(false);
  const [editando,   setEditando]   = useState<Sociedad | undefined>();
  const [toggling,   setToggling]   = useState<number | null>(null);
  const [uploading,  setUploading]  = useState(false);
  const fileRef = useRef<HTMLInputElement>(null);

  const cargar = async () => {
    setLoading(true);
    try {
      const res = await api.get('/sociedades', { params: soloActivas ? { soloActivas: true } : {} });
      setSociedades(res.data);
    } catch { toast.error('Error al cargar sociedades'); }
    finally { setLoading(false); }
  };

  useEffect(() => { cargar(); }, [soloActivas]);

  const handleToggle = async (s: Sociedad) => {
    setToggling(s.id);
    try {
      const res = await api.patch(`/sociedades/${s.id}/toggle`);
      setSociedades(prev => prev.map(x => x.id === s.id ? res.data : x));
      toast.success(`${res.data.codigo} ${res.data.activa ? 'activada' : 'desactivada'}`);
    } catch { toast.error('Error al cambiar estado'); }
    finally { setToggling(null); }
  };

  const handleSaved = (s: Sociedad) => {
    setSociedades(prev => {
      const idx = prev.findIndex(x => x.id === s.id);
      if (idx >= 0) { const n = [...prev]; n[idx] = s; return n; }
      return [...prev, s].sort((a, b) => a.codigo.localeCompare(b.codigo));
    });
  };

  const handleUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setUploading(true);
    try {
      const form = new FormData();
      form.append('archivo', file);
      const res = await api.post('/sociedades/cargar-excel', form, { headers: { 'Content-Type': 'multipart/form-data' } });
      const { insertadas, actualizadas, errores } = res.data;
      toast.success(`${insertadas} insertadas · ${actualizadas} actualizadas · ${errores} errores`);
      cargar();
    } catch { toast.error('Error al cargar el archivo'); }
    finally { setUploading(false); e.target.value = ''; }
  };

  const descargarPlantilla = async () => {
    const res = await api.get('/sociedades/plantilla', { responseType: 'blob' });
    const url = URL.createObjectURL(res.data);
    const a = document.createElement('a'); a.href = url; a.download = 'plantilla_sociedades.xlsx'; a.click();
    URL.revokeObjectURL(url);
  };

  const filtradas = sociedades.filter(s =>
    s.codigo.toLowerCase().includes(filtro.toLowerCase()) ||
    s.nombre.toLowerCase().includes(filtro.toLowerCase()) ||
    (s.pais ?? '').toLowerCase().includes(filtro.toLowerCase())
  );

  const byPais = filtradas.reduce<Record<string, Sociedad[]>>((acc, s) => {
    const p = s.pais ?? 'Sin país';
    if (!acc[p]) acc[p] = [];
    acc[p].push(s);
    return acc;
  }, {});

  const activas   = sociedades.filter(s => s.activa).length;
  const inactivas = sociedades.filter(s => !s.activa).length;

  return (
    <div className="p-6 max-w-5xl">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Mantenimiento de Sociedades</h1>
        <p className="text-sm text-gray-500 mt-0.5">Gestiona las sociedades SAP del grupo ILG Logistics</p>
      </div>

      <div className="grid grid-cols-3 gap-4 mb-6">
        {[
          { label: 'Total',     value: sociedades.length, color: 'text-gray-800',  bg: 'bg-gray-50'  },
          { label: 'Activas',   value: activas,           color: 'text-green-700', bg: 'bg-green-50' },
          { label: 'Inactivas', value: inactivas,         color: 'text-red-700',   bg: 'bg-red-50'   },
        ].map(k => (
          <div key={k.label} className={`${k.bg} rounded-xl border border-gray-100 px-5 py-4`}>
            <p className="text-xs text-gray-500">{k.label}</p>
            <p className={`text-3xl font-bold mt-1 ${k.color}`}>{k.value}</p>
          </div>
        ))}
      </div>

      <div className="flex items-center gap-3 mb-5 flex-wrap">
        <div className="relative flex-1 min-w-[200px]">
          <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input value={filtro} onChange={e => setFiltro(e.target.value)}
            placeholder="Buscar por código, nombre o país..."
            className="w-full pl-8 pr-3 py-2 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>
        <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer select-none">
          <input type="checkbox" checked={soloActivas} onChange={e => setSoloActivas(e.target.checked)} className="rounded" />
          Solo activas
        </label>
        <button onClick={() => { setEditando(undefined); setModalOpen(true); }}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 transition">
          <Plus size={15} /> Nueva
        </button>
        <input ref={fileRef} type="file" accept=".xlsx,.xls" className="hidden" onChange={handleUpload} />
        <button onClick={() => fileRef.current?.click()} disabled={uploading}
          className="flex items-center gap-2 border border-gray-200 text-gray-700 px-4 py-2 rounded-lg text-sm hover:bg-gray-50 transition disabled:opacity-50">
          {uploading ? <RefreshCw size={14} className="animate-spin" /> : <Upload size={14} />}
          Cargar Excel
        </button>
        <button onClick={descargarPlantilla}
          className="flex items-center gap-2 border border-gray-200 text-gray-700 px-4 py-2 rounded-lg text-sm hover:bg-gray-50 transition">
          <Download size={14} /> Plantilla
        </button>
        <button onClick={cargar} className="p-2 border border-gray-200 rounded-lg text-gray-500 hover:bg-gray-50 transition">
          <RefreshCw size={14} className={loading ? 'animate-spin' : ''} />
        </button>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-20 text-gray-400 gap-2">
          <RefreshCw size={18} className="animate-spin" /> Cargando...
        </div>
      ) : (
        <div className="space-y-6">
          {Object.entries(byPais).sort(([a], [b]) => a.localeCompare(b)).map(([pais, items]) => (
            <div key={pais}>
              <div className="flex items-center gap-2 mb-2">
                <Globe size={13} className="text-blue-500" />
                <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wide">{pais}</h3>
                <span className="text-xs text-gray-400">({items.length})</span>
              </div>
              <div className="border border-gray-200 rounded-xl overflow-hidden">
                <table className="w-full text-sm">
                  <thead className="bg-gray-50 border-b border-gray-200">
                    <tr>
                      <th className="text-left px-4 py-2.5 text-xs font-semibold text-gray-600 w-24">Código</th>
                      <th className="text-left px-4 py-2.5 text-xs font-semibold text-gray-600">Nombre</th>
                      <th className="text-center px-4 py-2.5 text-xs font-semibold text-gray-600 w-28">Estado</th>
                      <th className="px-4 py-2.5 w-24"></th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-100">
                    {items.map(s => (
                      <tr key={s.id} className={`hover:bg-gray-50 transition-colors ${!s.activa ? 'opacity-60' : ''}`}>
                        <td className="px-4 py-3">
                          <span className="font-mono font-semibold text-blue-700 bg-blue-50 px-2 py-0.5 rounded text-xs">{s.codigo}</span>
                        </td>
                        <td className="px-4 py-3 text-gray-800">{s.nombre}</td>
                        <td className="px-4 py-3 text-center">
                          {s.activa
                            ? <span className="inline-flex items-center gap-1 bg-green-100 text-green-700 px-2 py-0.5 rounded-full text-xs font-medium"><CheckCircle2 size={11} /> Activa</span>
                            : <span className="inline-flex items-center gap-1 bg-red-100 text-red-700 px-2 py-0.5 rounded-full text-xs font-medium"><XCircle size={11} /> Inactiva</span>}
                        </td>
                        <td className="px-4 py-3">
                          <div className="flex items-center justify-end gap-2">
                            <button onClick={() => { setEditando(s); setModalOpen(true); }}
                              className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded transition" title="Editar">
                              <Pencil size={13} />
                            </button>
                            <button onClick={() => handleToggle(s)} disabled={toggling === s.id}
                              className={`p-1.5 rounded transition ${s.activa ? 'text-gray-400 hover:text-red-600 hover:bg-red-50' : 'text-gray-400 hover:text-green-600 hover:bg-green-50'}`}
                              title={s.activa ? 'Desactivar' : 'Activar'}>
                              {toggling === s.id ? <RefreshCw size={13} className="animate-spin" /> : <Power size={13} />}
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          ))}
          {filtradas.length === 0 && (
            <div className="text-center py-16 text-gray-400 text-sm border border-dashed border-gray-200 rounded-xl">
              No se encontraron sociedades.
            </div>
          )}
        </div>
      )}

      {modalOpen && (
        <ModalSociedad
          sociedad={editando}
          onClose={() => { setModalOpen(false); setEditando(undefined); }}
          onSaved={handleSaved}
        />
      )}
    </div>
  );
}
