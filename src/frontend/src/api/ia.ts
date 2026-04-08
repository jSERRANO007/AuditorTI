import api from './client';

export interface IAResponse {
  respuesta: string;
  generadoAt: string;
}

export const consultarIA = (pregunta: string, contextoAdicional?: string): Promise<IAResponse> =>
  api.post('/ia/consultar', { pregunta, contextoAdicional }).then((r) => r.data);
