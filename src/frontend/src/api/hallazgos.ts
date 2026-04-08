import api from './client';
import type { PagedResult } from './simulaciones';

export interface HallazgoDto {
  id: string;
  titulo: string;
  descripcion: string;
  criticidad: string;
  estado: string;
  normaAfectada?: string;
  responsableEmail?: string;
  fechaCompromiso?: string;
  planAccion?: string;
  createdAt: string;
}

export const getHallazgos = (params?: {
  page?: number;
  pageSize?: number;
  estado?: string;
  criticidad?: string;
  sociedadId?: number;
}): Promise<PagedResult<HallazgoDto>> =>
  api.get('/hallazgos', { params }).then((r) => r.data);

export const actualizarPlanAccion = (
  id: string,
  data: { planAccion: string; fechaCompromiso?: string; responsableEmail?: string }
) => api.put(`/hallazgos/${id}/plan-accion`, data);

export const cerrarHallazgo = (id: string, justificacion: string) =>
  api.post(`/hallazgos/${id}/cerrar`, { justificacion });
