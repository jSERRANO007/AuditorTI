import api from './client';

export interface SimulacionListDto {
  id: string;
  nombre: string;
  estado: string;
  scoreMadurez?: number;
  porcentajeCumplimiento?: number;
  totalControles?: number;
  controlesRojo?: number;
  iniciadaPor: string;
  iniciadaAt: string;
  completadaAt?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface IniciarSimulacionRequest {
  nombre: string;
  descripcion?: string;
  tipo: 'MANUAL' | 'PROGRAMADA' | 'BAJO_DEMANDA';
  sociedadIds: number[];
  periodoInicio: string;
  periodoFin: string;
  dominioIds?: number[];
}

export const getSimulaciones = (page = 1, pageSize = 20): Promise<PagedResult<SimulacionListDto>> =>
  api.get('/simulaciones', { params: { page, pageSize } }).then((r) => r.data);

export const getSimulacion = (id: string) =>
  api.get(`/simulaciones/${id}`).then((r) => r.data);

export const iniciarSimulacion = (data: IniciarSimulacionRequest): Promise<{ id: string }> =>
  api.post('/simulaciones', data).then((r) => r.data);

export const cancelarSimulacion = (id: string) =>
  api.post(`/simulaciones/${id}/cancelar`);
