using System.Linq.Expressions;
using AuditorPRO.Domain.Entities;

namespace AuditorPRO.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface ISimulacionRepository : IRepository<SimulacionAuditoria>
{
    Task<SimulacionAuditoria?> GetWithResultadosAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<SimulacionAuditoria>> GetRecentesAsync(int count, CancellationToken ct = default);
}

public interface IHallazgoRepository : IRepository<Hallazgo>
{
    Task<IEnumerable<Hallazgo>> GetAbiertosAsync(CancellationToken ct = default);
    Task<IEnumerable<Hallazgo>> GetBySociedadAsync(int sociedadId, CancellationToken ct = default);
    Task<IEnumerable<Hallazgo>> GetBySimulacionAsync(Guid simulacionId, CancellationToken ct = default);
}

public interface IBitacoraRepository
{
    Task LogAsync(BitacoraEvento evento, CancellationToken ct = default);
    Task<IEnumerable<BitacoraEvento>> GetByUsuarioAsync(string usuarioId, int page, int size, CancellationToken ct = default);
}
