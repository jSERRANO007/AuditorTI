import { useState } from 'react';
import { consultarIA } from '../api/ia';
import { toast } from 'sonner';
import { Brain, Send } from 'lucide-react';

interface Mensaje { role: 'user' | 'assistant'; content: string; ts: Date }

const sugerencias = [
  '¿Cuáles son los controles críticos de SAP para la segregación de funciones?',
  'Dame un plan de acción para cerrar hallazgos de identidad',
  'Explica el control ISO 27001 A.9.2 en términos operativos',
  '¿Cómo evidencio la recertificación trimestral de usuarios?',
];

export function AgenteIA() {
  const [mensajes, setMensajes] = useState<Mensaje[]>([]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);

  const enviar = async (pregunta?: string) => {
    const texto = pregunta ?? input.trim();
    if (!texto) return;

    setInput('');
    setMensajes((prev) => [...prev, { role: 'user', content: texto, ts: new Date() }]);
    setLoading(true);

    try {
      const resp = await consultarIA(texto);
      setMensajes((prev) => [...prev, { role: 'assistant', content: resp.respuesta, ts: new Date() }]);
    } catch {
      toast.error('Error al consultar el agente IA');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6 flex flex-col h-full max-h-[calc(100vh-0px)]">
      <div className="flex items-center gap-2 mb-4">
        <Brain className="text-blue-600" size={24} />
        <h2 className="text-2xl font-bold text-gray-900">Agente IA Auditor</h2>
      </div>

      {mensajes.length === 0 && (
        <div className="mb-4">
          <p className="text-sm text-gray-500 mb-3">Preguntas sugeridas:</p>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-2">
            {sugerencias.map((s) => (
              <button
                key={s}
                onClick={() => enviar(s)}
                className="text-left text-sm bg-blue-50 text-blue-700 border border-blue-200 rounded-lg px-3 py-2 hover:bg-blue-100 transition"
              >
                {s}
              </button>
            ))}
          </div>
        </div>
      )}

      <div className="flex-1 overflow-auto bg-gray-50 rounded-xl border p-4 space-y-4 mb-4">
        {mensajes.map((m, i) => (
          <div key={i} className={`flex ${m.role === 'user' ? 'justify-end' : 'justify-start'}`}>
            <div
              className={`max-w-[80%] rounded-xl px-4 py-3 text-sm ${
                m.role === 'user'
                  ? 'bg-blue-600 text-white'
                  : 'bg-white border text-gray-800 shadow-sm'
              }`}
            >
              {m.role === 'assistant' && (
                <div className="flex items-center gap-1.5 mb-2">
                  <Brain size={12} className="text-blue-600" />
                  <span className="text-xs font-medium text-blue-600">Agente IA</span>
                </div>
              )}
              <p className="whitespace-pre-wrap">{m.content}</p>
            </div>
          </div>
        ))}
        {loading && (
          <div className="flex justify-start">
            <div className="bg-white border rounded-xl px-4 py-3 shadow-sm">
              <div className="flex gap-1">
                <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce" />
                <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce [animation-delay:0.1s]" />
                <div className="w-2 h-2 bg-blue-400 rounded-full animate-bounce [animation-delay:0.2s]" />
              </div>
            </div>
          </div>
        )}
      </div>

      <div className="flex gap-2">
        <input
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyDown={(e) => { if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); enviar(); }}}
          placeholder="Consulta al agente IA sobre controles, normas, planes de acción..."
          className="flex-1 border rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          disabled={loading}
        />
        <button
          onClick={() => enviar()}
          disabled={loading || !input.trim()}
          className="bg-blue-600 text-white rounded-xl px-4 py-2.5 hover:bg-blue-700 disabled:opacity-50 transition"
        >
          <Send size={16} />
        </button>
      </div>
    </div>
  );
}
