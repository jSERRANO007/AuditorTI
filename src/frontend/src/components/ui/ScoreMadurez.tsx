interface ScoreMadurezProps {
  score: number; // 0-10
}

function getColor(score: number) {
  if (score >= 8) return '#22c55e';
  if (score >= 6) return '#eab308';
  if (score >= 4) return '#f97316';
  return '#ef4444';
}

function getLabel(score: number) {
  if (score >= 9) return 'Óptimo';
  if (score >= 7) return 'Gestionado';
  if (score >= 5) return 'Definido';
  if (score >= 3) return 'Repetible';
  return 'Inicial';
}

export function ScoreMadurez({ score }: ScoreMadurezProps) {
  const pct = (score / 10) * 100;
  const color = getColor(score);

  return (
    <div className="text-center">
      <div className="relative inline-flex items-center justify-center w-28 h-28">
        <svg className="w-28 h-28 -rotate-90" viewBox="0 0 100 100">
          <circle cx="50" cy="50" r="42" fill="none" stroke="#e5e7eb" strokeWidth="8" />
          <circle
            cx="50" cy="50" r="42" fill="none"
            stroke={color} strokeWidth="8"
            strokeDasharray={`${2 * Math.PI * 42}`}
            strokeDashoffset={`${2 * Math.PI * 42 * (1 - pct / 100)}`}
            strokeLinecap="round"
          />
        </svg>
        <div className="absolute flex flex-col items-center">
          <span className="text-2xl font-bold" style={{ color }}>{score.toFixed(1)}</span>
          <span className="text-xs text-gray-500">/10</span>
        </div>
      </div>
      <p className="text-sm font-medium text-gray-600 mt-1">{getLabel(score)}</p>
    </div>
  );
}
