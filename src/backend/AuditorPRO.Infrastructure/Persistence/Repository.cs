using System.Linq.Expressions;
using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditorPRO.Infrastructure.Persistence;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.ToListAsync(ct);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.Where(predicate).ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}

public class SimulacionRepository : Repository<SimulacionAuditoria>, ISimulacionRepository
{
    public SimulacionRepository(AppDbContext context) : base(context) { }

    public async Task<SimulacionAuditoria?> GetWithResultadosAsync(Guid id, CancellationToken ct = default)
        => await _context.Simulaciones
            .Include(s => s.Resultados)
                .ThenInclude(r => r.PuntoControl)
                    .ThenInclude(p => p.Dominio)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<SimulacionAuditoria>> GetRecentesAsync(int count, CancellationToken ct = default)
        => await _context.Simulaciones
            .OrderByDescending(s => s.IniciadaAt)
            .Take(count)
            .ToListAsync(ct);
}

public class HallazgoRepository : Repository<Hallazgo>, IHallazgoRepository
{
    public HallazgoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Hallazgo>> GetAbiertosAsync(CancellationToken ct = default)
        => await _context.Hallazgos
            .Include(h => h.Sociedad)
            .Where(h => h.Estado == Domain.Enums.EstadoHallazgo.ABIERTO || h.Estado == Domain.Enums.EstadoHallazgo.EN_PROCESO)
            .OrderByDescending(h => h.Criticidad)
            .ToListAsync(ct);

    public async Task<IEnumerable<Hallazgo>> GetBySociedadAsync(int sociedadId, CancellationToken ct = default)
        => await _context.Hallazgos
            .Where(h => h.SociedadId == sociedadId)
            .ToListAsync(ct);
}

public class BitacoraRepository : IBitacoraRepository
{
    private readonly AppDbContext _context;

    public BitacoraRepository(AppDbContext context) => _context = context;

    public async Task LogAsync(BitacoraEvento evento, CancellationToken ct = default)
    {
        await _context.Bitacora.AddAsync(evento, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<BitacoraEvento>> GetByUsuarioAsync(string usuarioId, int page, int size, CancellationToken ct = default)
        => await _context.Bitacora
            .Where(b => b.UsuarioId == usuarioId)
            .OrderByDescending(b => b.OcurridoAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);
}
