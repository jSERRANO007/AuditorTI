using AuditorPRO.Domain.Common;
using AuditorPRO.Domain.Enums;

namespace AuditorPRO.Domain.Entities;

public class Conector : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TipoConector TipoConector { get; set; }
    public string Sistema { get; set; } = string.Empty;
    public EstadoConector Estado { get; set; } = EstadoConector.ACTIVO;
    public string ConfiguracionJson { get; set; } = "{}";
    public string? UrlEndpoint { get; set; }
    public string? AuthType { get; set; }
    public string? SecretKeyVaultRef { get; set; }
    public DateTime? UltimaEjecucion { get; set; }
    public bool UltimaEjecucionExito { get; set; } = false;
    public string? UltimoError { get; set; }
    public int TotalEjecuciones { get; set; } = 0;
    public ICollection<LogConector> Logs { get; set; } = [];
}

public class LogConector
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConectorId { get; set; }
    public Conector Conector { get; set; } = null!;
    public bool Exitoso { get; set; }
    public int? RegistrosProcesados { get; set; }
    public string? MensajeError { get; set; }
    public int DuracionMs { get; set; }
    public DateTime EjecutadoAt { get; set; } = DateTime.UtcNow;
    public string? EjecutadoPor { get; set; }
}
