interface KpiTileProps {
  label: string;
  value: string | number;
  sublabel?: string;
  color?: 'blue' | 'red' | 'green' | 'yellow' | 'gray';
  icon?: React.ReactNode;
}

const colorMap = {
  blue: 'bg-blue-50 border-blue-200 text-blue-700',
  red: 'bg-red-50 border-red-200 text-red-700',
  green: 'bg-green-50 border-green-200 text-green-700',
  yellow: 'bg-yellow-50 border-yellow-200 text-yellow-700',
  gray: 'bg-gray-50 border-gray-200 text-gray-700',
};

export function KpiTile({ label, value, sublabel, color = 'blue', icon }: KpiTileProps) {
  return (
    <div className={`rounded-xl border-2 p-4 ${colorMap[color]}`}>
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm font-medium opacity-70">{label}</p>
          <p className="text-3xl font-bold mt-1">{value}</p>
          {sublabel && <p className="text-xs mt-1 opacity-60">{sublabel}</p>}
        </div>
        {icon && <div className="opacity-60">{icon}</div>}
      </div>
    </div>
  );
}
