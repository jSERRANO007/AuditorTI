import api from './client';

export interface DashboardDto {
  scoreMadurezGlobal: number;
  porcentajeCumplimiento: number;
  totalHallazgosAbiertos: number;
  hallazgosCriticos: number;
  hallazgosMedios: number;
  hallazgosBajos: number;
  simulacionesUltimos30Dias: number;
  semaforoResumen: { verde: number; amarillo: number; rojo: number; total: number };
  tendenciaUltimos6Meses: { periodo: string; score: number; hallazgos: number }[];
  topHallazgosCriticos: {
    id: string;
    titulo: string;
    criticidad: string;
    estado: string;
    fechaCompromiso?: string;
  }[];
  puntajePorDominio: { dominio: string; score: number; totalControles: number; rojo: number }[];
}

export const getDashboard = (sociedadId?: number): Promise<DashboardDto> =>
  api.get('/dashboard', { params: { sociedadId } }).then((r) => r.data);
