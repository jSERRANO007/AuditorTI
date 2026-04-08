using AuditorPRO.Domain.Common;
using AuditorPRO.Domain.Enums;

namespace AuditorPRO.Domain.Entities;

public class Hallazgo : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? SimulacionId { get; set; }
    public SimulacionAuditoria? Simulacion { get; set; }
    public Guid? ResultadoControlId { get; set; }
    public ResultadoControl? ResultadoControl { get; set; }
    public int? SociedadId { get; set; }
    public Sociedad? Sociedad { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Criticidad Criticidad { get; set; }
    public EstadoHallazgo Estado { get; set; } = EstadoHallazgo.ABIERTO;
    public string? NormaAfectada { get; set; }
    public string? RiesgoAsociado { get; set; }
    public string? ResponsableEmail { get; set; }
    public DateOnly? FechaCompromiso { get; set; }
    public DateOnly? FechaCierre { get; set; }
    public string? PlanAccion { get; set; }
    public string? AnalisisIa { get; set; }
    public string? EvidenciaCierreIds { get; set; }
    public ICollection<Evidencia> Evidencias { get; set; } = [];
}

public class Evidencia : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? HallazgoId { get; set; }
    public Hallazgo? Hallazgo { get; set; }
    public Guid? SimulacionId { get; set; }
    public TipoEvidencia TipoEvidencia { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string? DescripcionArchivo { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public string? BlobContainer { get; set; }
    public long TamanoBytes { get; set; }
    public string? ContentType { get; set; }
    public string SubidoPor { get; set; } = string.Empty;
    public DateTime SubidoAt { get; set; } = DateTime.UtcNow;
    public bool Verificada { get; set; } = false;
    public string? HashSha256 { get; set; }
}
