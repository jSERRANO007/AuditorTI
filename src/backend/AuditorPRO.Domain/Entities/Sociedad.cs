using AuditorPRO.Domain.Common;

namespace AuditorPRO.Domain.Entities;

public class Sociedad : BaseEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Pais { get; set; }
    public bool Activa { get; set; } = true;

    public ICollection<Departamento> Departamentos { get; set; } = [];
    public ICollection<Puesto> Puestos { get; set; } = [];
    public ICollection<EmpleadoMaestro> Empleados { get; set; } = [];
}

public class Departamento : BaseEntity
{
    public int Id { get; set; }
    public int SociedadId { get; set; }
    public Sociedad Sociedad { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class Puesto : BaseEntity
{
    public int Id { get; set; }
    public int SociedadId { get; set; }
    public Sociedad Sociedad { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? NivelRiesgo { get; set; }
    public bool Activo { get; set; } = true;
}
