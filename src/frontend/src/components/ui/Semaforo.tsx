interface SemaforoProps {
  valor: 'VERDE' | 'AMARILLO' | 'ROJO' | 'NO_EVALUADO';
  size?: 'sm' | 'md' | 'lg';
  showLabel?: boolean;
}

const colors = {
  VERDE: 'bg-green-500 text-green-800',
  AMARILLO: 'bg-yellow-400 text-yellow-900',
  ROJO: 'bg-red-500 text-white',
  NO_EVALUADO: 'bg-gray-300 text-gray-700',
};

const labels = {
  VERDE: 'Cumple',
  AMARILLO: 'Observación',
  ROJO: 'No cumple',
  NO_EVALUADO: 'Sin evaluar',
};

const sizes = {
  sm: 'w-3 h-3',
  md: 'w-4 h-4',
  lg: 'w-6 h-6',
};

export function Semaforo({ valor, size = 'md', showLabel = false }: SemaforoProps) {
  return (
    <span className="inline-flex items-center gap-1.5">
      <span className={`rounded-full inline-block ${sizes[size]} ${colors[valor].split(' ')[0]}`} />
      {showLabel && (
        <span className={`text-xs font-medium ${colors[valor].split(' ')[1]}`}>
          {labels[valor]}
        </span>
      )}
    </span>
  );
}
