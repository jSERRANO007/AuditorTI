using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Interfaces;
using ClosedXML.Excel;
using MediatR;

namespace AuditorPRO.Application.Features.Sociedades;

// ── Crear ────────────────────────────────────────────────────────────────────
public record CrearSociedadCommand(string Codigo, string Nombre, string? Pais) : IRequest<SociedadDto>;

public class CrearSociedadHandler : IRequestHandler<CrearSociedadCommand, SociedadDto>
{
    private readonly IRepository<Sociedad> _repo;
    public CrearSociedadHandler(IRepository<Sociedad> repo) => _repo = repo;

    public async Task<SociedadDto> Handle(CrearSociedadCommand req, CancellationToken ct)
    {
        var existentes = await _repo.FindAsync(s => s.Codigo == req.Codigo.ToUpperInvariant(), ct);
        if (existentes.Any())
            throw new InvalidOperationException($"Ya existe una sociedad con código {req.Codigo}.");

        var s = new Sociedad
        {
            Codigo = req.Codigo.ToUpperInvariant(),
            Nombre = req.Nombre.Trim(),
            Pais   = req.Pais?.Trim(),
            Activa = true,
        };
        await _repo.AddAsync(s, ct);
        await _repo.SaveChangesAsync(ct);

        return new SociedadDto { Id = s.Id, Codigo = s.Codigo, Nombre = s.Nombre, Pais = s.Pais, Activa = s.Activa };
    }
}

// ── Actualizar ───────────────────────────────────────────────────────────────
public record ActualizarSociedadCommand(int Id, string Nombre, string? Pais) : IRequest<SociedadDto>;

public class ActualizarSociedadHandler : IRequestHandler<ActualizarSociedadCommand, SociedadDto>
{
    private readonly IRepository<Sociedad> _repo;
    public ActualizarSociedadHandler(IRepository<Sociedad> repo) => _repo = repo;

    public async Task<SociedadDto> Handle(ActualizarSociedadCommand req, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync(req.Id, ct)
            ?? throw new KeyNotFoundException($"Sociedad {req.Id} no encontrada.");

        s.Nombre    = req.Nombre.Trim();
        s.Pais      = req.Pais?.Trim();
        s.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync(ct);

        return new SociedadDto { Id = s.Id, Codigo = s.Codigo, Nombre = s.Nombre, Pais = s.Pais, Activa = s.Activa };
    }
}

// ── Activar / Desactivar ─────────────────────────────────────────────────────
public record ToggleSociedadCommand(int Id) : IRequest<SociedadDto>;

public class ToggleSociedadHandler : IRequestHandler<ToggleSociedadCommand, SociedadDto>
{
    private readonly IRepository<Sociedad> _repo;
    public ToggleSociedadHandler(IRepository<Sociedad> repo) => _repo = repo;

    public async Task<SociedadDto> Handle(ToggleSociedadCommand req, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync(req.Id, ct)
            ?? throw new KeyNotFoundException($"Sociedad {req.Id} no encontrada.");

        s.Activa    = !s.Activa;
        s.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync(ct);

        return new SociedadDto { Id = s.Id, Codigo = s.Codigo, Nombre = s.Nombre, Pais = s.Pais, Activa = s.Activa };
    }
}

// ── Carga masiva desde Excel ─────────────────────────────────────────────────
public record CargarSociedadesCommand(Stream Contenido) : IRequest<CargaSociedadesResultado>;

public record CargaSociedadesResultado(int Insertadas, int Actualizadas, int Errores, List<string> Detalles);

public class CargarSociedadesHandler : IRequestHandler<CargarSociedadesCommand, CargaSociedadesResultado>
{
    private readonly IRepository<Sociedad> _repo;
    public CargarSociedadesHandler(IRepository<Sociedad> repo) => _repo = repo;

    public async Task<CargaSociedadesResultado> Handle(CargarSociedadesCommand req, CancellationToken ct)
    {
        using var wb = new XLWorkbook(req.Contenido);
        var ws = wb.Worksheet(1);
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

        var existentes = (await _repo.GetAllAsync(ct)).ToDictionary(s => s.Codigo.ToUpperInvariant());
        int insertadas = 0, actualizadas = 0, errores = 0;
        var detalles = new List<string>();

        for (int r = 2; r <= lastRow; r++)
        {
            var codigo = ws.Cell(r, 1).GetString().Trim().ToUpperInvariant();
            var nombre = ws.Cell(r, 2).GetString().Trim();
            var pais   = ws.Cell(r, 3).GetString().Trim();
            var activaStr = ws.Cell(r, 4).GetString().Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre)) continue;

            bool activa = activaStr is "" or "ACTIVA" or "TRUE" or "SI" or "SÍ" or "1";

            try
            {
                if (existentes.TryGetValue(codigo, out var s))
                {
                    s.Nombre = nombre;
                    s.Pais   = string.IsNullOrWhiteSpace(pais) ? s.Pais : pais;
                    s.Activa = activa;
                    s.UpdatedAt = DateTime.UtcNow;
                    actualizadas++;
                }
                else
                {
                    await _repo.AddAsync(new Sociedad { Codigo = codigo, Nombre = nombre, Pais = pais, Activa = activa }, ct);
                    insertadas++;
                }
            }
            catch (Exception ex)
            {
                errores++;
                detalles.Add($"Fila {r} ({codigo}): {ex.Message}");
            }
        }

        await _repo.SaveChangesAsync(ct);
        return new CargaSociedadesResultado(insertadas, actualizadas, errores, detalles);
    }
}
