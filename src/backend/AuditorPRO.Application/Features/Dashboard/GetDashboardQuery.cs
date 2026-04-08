using AuditorPRO.Domain.Enums;
using AuditorPRO.Domain.Interfaces;
using MediatR;

namespace AuditorPRO.Application.Features.Dashboard;

public record GetDashboardQuery(int? SociedadId = null) : IRequest<DashboardDto>;

public class DashboardDto
{
    public decimal ScoreMadurezGlobal { get; set; }
    public decimal PorcentajeCumplimiento { get; set; }
    public int TotalHallazgosAbiertos { get; set; }
    public int HallazgosCriticos { get; set; }
    public int HallazgosMedios { get; set; }
    public int HallazgosBajos { get; set; }
    public int SimulacionesUltimos30Dias { get; set; }
    public SemaforoResumenDto SemaforoResumen { get; set; } = new();
    public IEnumerable<TendenciaDto> TendenciaUltimos6Meses { get; set; } = [];
    public IEnumerable<HallazgoCriticoDto> TopHallazgosCriticos { get; set; } = [];
    public IEnumerable<DominioPuntajeDto> PuntajePorDominio { get; set; } = [];
}

public class SemaforoResumenDto
{
    public int Verde { get; set; }
    public int Amarillo { get; set; }
    public int Rojo { get; set; }
    public int Total { get; set; }
}

public class TendenciaDto
{
    public string Periodo { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public int Hallazgos { get; set; }
}

public class HallazgoCriticoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Criticidad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Sociedad { get; set; }
    public DateOnly? FechaCompromiso { get; set; }
}

public class DominioPuntajeDto
{
    public string Dominio { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public int TotalControles { get; set; }
    public int Rojo { get; set; }
}

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly ISimulacionRepository _simulacionRepo;
    private readonly IHallazgoRepository _hallazgoRepo;

    public GetDashboardQueryHandler(ISimulacionRepository simulacionRepo, IHallazgoRepository hallazgoRepo)
    {
        _simulacionRepo = simulacionRepo;
        _hallazgoRepo = hallazgoRepo;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var simulacionesRecientes = await _simulacionRepo.GetRecentesAsync(6, cancellationToken);
        var hallazgosAbiertos = await _hallazgoRepo.GetAbiertosAsync(cancellationToken);

        var ultimaSimulacion = simulacionesRecientes.FirstOrDefault();
        var hallazgosList = hallazgosAbiertos.ToList();

        return new DashboardDto
        {
            ScoreMadurezGlobal = ultimaSimulacion?.ScoreMadurez ?? 0,
            PorcentajeCumplimiento = ultimaSimulacion?.PorcentajeCumplimiento ?? 0,
            TotalHallazgosAbiertos = hallazgosList.Count,
            HallazgosCriticos = hallazgosList.Count(h => h.Criticidad == Criticidad.CRITICA),
            HallazgosMedios = hallazgosList.Count(h => h.Criticidad == Criticidad.MEDIA),
            HallazgosBajos = hallazgosList.Count(h => h.Criticidad == Criticidad.BAJA),
            SimulacionesUltimos30Dias = simulacionesRecientes.Count(s => s.IniciadaAt >= DateTime.UtcNow.AddDays(-30)),
            SemaforoResumen = new SemaforoResumenDto
            {
                Verde = ultimaSimulacion?.ControlesVerde ?? 0,
                Amarillo = ultimaSimulacion?.ControlesAmarillo ?? 0,
                Rojo = ultimaSimulacion?.ControlesRojo ?? 0,
                Total = ultimaSimulacion?.TotalControles ?? 0
            },
            TendenciaUltimos6Meses = simulacionesRecientes.Select(s => new TendenciaDto
            {
                Periodo = s.IniciadaAt.ToString("MMM yyyy"),
                Score = s.ScoreMadurez ?? 0,
                Hallazgos = s.ControlesRojo ?? 0
            }),
            TopHallazgosCriticos = hallazgosList
                .Where(h => h.Criticidad == Criticidad.CRITICA)
                .Take(5)
                .Select(h => new HallazgoCriticoDto
                {
                    Id = h.Id,
                    Titulo = h.Titulo,
                    Criticidad = h.Criticidad.ToString(),
                    Estado = h.Estado.ToString(),
                    FechaCompromiso = h.FechaCompromiso
                })
        };
    }
}
