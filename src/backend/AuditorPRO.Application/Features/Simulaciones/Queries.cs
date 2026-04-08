using AuditorPRO.Application.Common.Models;
using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Enums;
using AuditorPRO.Domain.Interfaces;
using MediatR;

namespace AuditorPRO.Application.Features.Simulaciones;

public record GetSimulacionesQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<SimulacionListDto>>;

public class SimulacionListDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal? ScoreMadurez { get; set; }
    public decimal? PorcentajeCumplimiento { get; set; }
    public int? TotalControles { get; set; }
    public int? ControlesRojo { get; set; }
    public string IniciadaPor { get; set; } = string.Empty;
    public DateTime IniciadaAt { get; set; }
    public DateTime? CompletadaAt { get; set; }
}

public class GetSimulacionesHandler : IRequestHandler<GetSimulacionesQuery, PagedResult<SimulacionListDto>>
{
    private readonly ISimulacionRepository _repo;

    public GetSimulacionesHandler(ISimulacionRepository repo) => _repo = repo;

    public async Task<PagedResult<SimulacionListDto>> Handle(GetSimulacionesQuery request, CancellationToken cancellationToken)
    {
        var all = (await _repo.GetAllAsync(cancellationToken)).ToList();
        var total = all.Count;
        var items = all
            .OrderByDescending(s => s.IniciadaAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SimulacionListDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Estado = s.Estado.ToString(),
                ScoreMadurez = s.ScoreMadurez,
                PorcentajeCumplimiento = s.PorcentajeCumplimiento,
                TotalControles = s.TotalControles,
                ControlesRojo = s.ControlesRojo,
                IniciadaPor = s.IniciadaPor,
                IniciadaAt = s.IniciadaAt,
                CompletadaAt = s.CompletadaAt
            });

        return new PagedResult<SimulacionListDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

public record GetSimulacionDetalleQuery(Guid Id) : IRequest<SimulacionDetalleDto?>;

public class SimulacionDetalleDto : SimulacionListDto
{
    public string? Descripcion { get; set; }
    public DateOnly PeriodoInicio { get; set; }
    public DateOnly PeriodoFin { get; set; }
    public int? ControlesVerde { get; set; }
    public int? ControlesAmarillo { get; set; }
    public IEnumerable<ResultadoControlDto> Resultados { get; set; } = [];
}

public class ResultadoControlDto
{
    public Guid Id { get; set; }
    public string CodigoControl { get; set; } = string.Empty;
    public string NombreControl { get; set; } = string.Empty;
    public string Dominio { get; set; } = string.Empty;
    public string Semaforo { get; set; } = string.Empty;
    public string Criticidad { get; set; } = string.Empty;
    public string? ResultadoDetalle { get; set; }
    public string? AnalisisIa { get; set; }
    public string? Recomendacion { get; set; }
}

public class GetSimulacionDetalleHandler : IRequestHandler<GetSimulacionDetalleQuery, SimulacionDetalleDto?>
{
    private readonly ISimulacionRepository _repo;

    public GetSimulacionDetalleHandler(ISimulacionRepository repo) => _repo = repo;

    public async Task<SimulacionDetalleDto?> Handle(GetSimulacionDetalleQuery request, CancellationToken cancellationToken)
    {
        var sim = await _repo.GetWithResultadosAsync(request.Id, cancellationToken);
        if (sim == null) return null;

        return new SimulacionDetalleDto
        {
            Id = sim.Id,
            Nombre = sim.Nombre,
            Descripcion = sim.Descripcion,
            Estado = sim.Estado.ToString(),
            ScoreMadurez = sim.ScoreMadurez,
            PorcentajeCumplimiento = sim.PorcentajeCumplimiento,
            TotalControles = sim.TotalControles,
            ControlesVerde = sim.ControlesVerde,
            ControlesAmarillo = sim.ControlesAmarillo,
            ControlesRojo = sim.ControlesRojo,
            IniciadaPor = sim.IniciadaPor,
            IniciadaAt = sim.IniciadaAt,
            CompletadaAt = sim.CompletadaAt,
            PeriodoInicio = sim.PeriodoInicio,
            PeriodoFin = sim.PeriodoFin,
            Resultados = sim.Resultados.Select(r => new ResultadoControlDto
            {
                Id = r.Id,
                CodigoControl = r.PuntoControl?.Codigo ?? "",
                NombreControl = r.PuntoControl?.Nombre ?? "",
                Dominio = r.PuntoControl?.Dominio?.Nombre ?? "",
                Semaforo = r.Semaforo.ToString(),
                Criticidad = r.Criticidad.ToString(),
                ResultadoDetalle = r.ResultadoDetalle,
                AnalisisIa = r.AnalisisIa,
                Recomendacion = r.Recomendacion
            })
        };
    }
}
