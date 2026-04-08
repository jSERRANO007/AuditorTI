using AuditorPRO.Domain.Common;
using AuditorPRO.Domain.Enums;

namespace AuditorPRO.Domain.Entities;

public class EmpleadoMaestro : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NumeroEmpleado { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? CorreoCorporativo { get; set; }
    public string? EntraIdObject { get; set; }
    public int? SociedadId { get; set; }
    public Sociedad? Sociedad { get; set; }
    public int? DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }
    public int? PuestoId { get; set; }
    public Puesto? Puesto { get; set; }
    public Guid? JefeEmpleadoId { get; set; }
    public EmpleadoMaestro? JefeEmpleado { get; set; }
    public EstadoLaboral EstadoLaboral { get; set; }
    public DateOnly? FechaIngreso { get; set; }
    public DateOnly? FechaBaja { get; set; }
    public string? FuenteOrigen { get; set; }
    public Guid? LoteCargaId { get; set; }
}

public class UsuarioSistema : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Sistema { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public Guid? EmpleadoId { get; set; }
    public EmpleadoMaestro? Empleado { get; set; }
    public EstadoUsuario Estado { get; set; }
    public string? TipoUsuario { get; set; }
    public DateTime? FechaUltimoAcceso { get; set; }
    public string? FuenteOrigen { get; set; }
}
