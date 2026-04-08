using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Enums;
using AuditorPRO.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AuditorPRO.Application.Features.Simulaciones;

// --- Commands ---

public record IniciarSimulacionCommand(
    string Nombre,
    string? Descripcion,
    TipoSimulacion Tipo,
    List<int> SociedadIds,
    DateOnly PeriodoInicio,
    DateOnly PeriodoFin,
    List<int>? DominioIds,
    List<int>? PuntosControlIds
) : IRequest<Guid>;

public class IniciarSimulacionValidator : AbstractValidator<IniciarSimulacionCommand>
{
    public IniciarSimulacionValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(300);
        RuleFor(x => x.SociedadIds).NotEmpty().WithMessage("Debe seleccionar al menos una sociedad.");
        RuleFor(x => x.PeriodoInicio).LessThan(x => x.PeriodoFin);
    }
}

public class IniciarSimulacionHandler : IRequestHandler<IniciarSimulacionCommand, Guid>
{
    private readonly ISimulacionRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLoggerService _auditLogger;

    public IniciarSimulacionHandler(ISimulacionRepository repo, ICurrentUserService currentUser, IAuditLoggerService auditLogger)
    {
        _repo = repo;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
    }

    public async Task<Guid> Handle(IniciarSimulacionCommand request, CancellationToken cancellationToken)
    {
        var simulacion = new SimulacionAuditoria
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Tipo = request.Tipo,
            Estado = EstadoSimulacion.PENDIENTE,
            SociedadIds = System.Text.Json.JsonSerializer.Serialize(request.SociedadIds),
            PeriodoInicio = request.PeriodoInicio,
            PeriodoFin = request.PeriodoFin,
            DominioIds = request.DominioIds != null ? System.Text.Json.JsonSerializer.Serialize(request.DominioIds) : null,
            PuntosControlIds = request.PuntosControlIds != null ? System.Text.Json.JsonSerializer.Serialize(request.PuntosControlIds) : null,
            IniciadaPor = _currentUser.Email,
            CreatedBy = _currentUser.Email
        };

        await _repo.AddAsync(simulacion, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(_currentUser.UserId, _currentUser.Email,
            "EJECUTAR_SIMULACION", "SimulacionAuditoria", simulacion.Id.ToString(), ct: cancellationToken);

        return simulacion.Id;
    }
}

public record CancelarSimulacionCommand(Guid SimulacionId) : IRequest;

public class CancelarSimulacionHandler : IRequestHandler<CancelarSimulacionCommand>
{
    private readonly ISimulacionRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public CancelarSimulacionHandler(ISimulacionRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelarSimulacionCommand request, CancellationToken cancellationToken)
    {
        var sim = await _repo.GetByIdAsync(request.SimulacionId, cancellationToken)
            ?? throw new KeyNotFoundException($"Simulación {request.SimulacionId} no encontrada.");

        if (sim.Estado == EstadoSimulacion.COMPLETADA)
            throw new InvalidOperationException("No se puede cancelar una simulación completada.");

        sim.Estado = EstadoSimulacion.CANCELADA;
        await _repo.UpdateAsync(sim, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}
