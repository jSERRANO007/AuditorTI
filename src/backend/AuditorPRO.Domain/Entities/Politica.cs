using AuditorPRO.Domain.Common;
using AuditorPRO.Domain.Enums;

namespace AuditorPRO.Domain.Entities;

public class Politica : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoPolitica Estado { get; set; } = EstadoPolitica.BORRADOR;
    public string? NormaReferencia { get; set; }
    public string? Responsable { get; set; }
    public DateOnly? FechaVigencia { get; set; }
    public DateOnly? FechaRevision { get; set; }
    public int Version { get; set; } = 1;
    public string? DocumentoUrl { get; set; }
    public string? Contenido { get; set; }
}

public class BitacoraEvento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UsuarioId { get; set; } = string.Empty;
    public string? UsuarioEmail { get; set; }
    public AccionBitacora Accion { get; set; }
    public string Recurso { get; set; } = string.Empty;
    public string? RecursoId { get; set; }
    public string? Descripcion { get; set; }
    public string? DatosAntes { get; set; }
    public string? DatosDespues { get; set; }
    public string? IpOrigen { get; set; }
    public string? UserAgent { get; set; }
    public bool Exitoso { get; set; } = true;
    public string? ErrorDetalle { get; set; }
    public DateTime OcurridoAt { get; set; } = DateTime.UtcNow;
}
