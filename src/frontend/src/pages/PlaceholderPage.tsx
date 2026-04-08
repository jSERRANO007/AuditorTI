interface Props { title: string; description?: string }

export function PlaceholderPage({ title, description }: Props) {
  return (
    <div className="p-6">
      <h2 className="text-2xl font-bold text-gray-900">{title}</h2>
      {description && <p className="text-gray-500 mt-1">{description}</p>}
      <div className="mt-8 bg-white rounded-xl border p-8 text-center text-gray-400">
        Módulo en desarrollo — próxima versión
      </div>
    </div>
  );
}
