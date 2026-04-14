using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Interfaces;
using MediatR;

namespace AuditorPRO.Application.Features.Cargas;

public record MatrizPuestoDto(
    string UsuarioSAP,
    string? NombreCompleto,
    string? Cedula,
    string? Sociedad,
    string? Departamento,
    string? Puesto,
    string? Email,
    string Rol,
    string? Transaccion,
    string? InicioValidez,
    string? FinValidez,
    string? UltimoIngreso,
    string? FechaRevisionContraloria
);

public record MatrizPuestosResultado(
    int Total,
    int Page,
    int PageSize,
    List<MatrizPuestoDto> Items
);

public record GetMatrizPuestosQuery(
    string? Usuario,
    string? Puesto,
    string? Rol,
    string? Transaccion,
    int Page,
    int PageSize
) : IRequest<MatrizPuestosResultado>;

public class GetMatrizPuestosHandler : IRequestHandler<GetMatrizPuestosQuery, MatrizPuestosResultado>
{
    private readonly IRepository<MatrizPuestoSAP> _repo;

    public GetMatrizPuestosHandler(IRepository<MatrizPuestoSAP> repo) => _repo = repo;

    public async Task<MatrizPuestosResultado> Handle(GetMatrizPuestosQuery request, CancellationToken ct)
    {
        var todos = await _repo.GetAllAsync(ct);

        var query = todos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Usuario))
            query = query.Where(m => m.UsuarioSAP.Contains(request.Usuario, StringComparison.OrdinalIgnoreCase)
                                  || (m.NombreCompleto != null && m.NombreCompleto.Contains(request.Usuario, StringComparison.OrdinalIgnoreCase)));

        if (!string.IsNullOrWhiteSpace(request.Puesto))
            query = query.Where(m => m.Puesto.Contains(request.Puesto, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Rol))
            query = query.Where(m => m.Rol.Contains(request.Rol, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Transaccion))
            query = query.Where(m => m.Transaccion != null && m.Transaccion.Contains(request.Transaccion, StringComparison.OrdinalIgnoreCase));

        var total = query.Count();
        var items = query
            .OrderBy(m => m.UsuarioSAP).ThenBy(m => m.Rol).ThenBy(m => m.Transaccion)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList()
            .Select(m => new MatrizPuestoDto(
                m.UsuarioSAP,
                m.NombreCompleto,
                m.Cedula,
                m.Sociedad,
                m.Departamento,
                m.Puesto,
                m.Email,
                m.Rol,
                m.Transaccion,
                m.InicioValidez?.ToString("yyyy-MM-dd"),
                m.FinValidez?.ToString("yyyy-MM-dd"),
                m.UltimoIngreso?.ToString("yyyy-MM-dd"),
                m.FechaRevisionContraloria?.ToString("yyyy-MM-dd")
            ))
            .ToList();

        return new MatrizPuestosResultado(total, request.Page, request.PageSize, items);
    }
}
