using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Enums;
using AuditorPRO.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AuditorPRO.Application.Features.Hallazgos;

public record ActualizarPlanAccionCommand(
    Guid HallazgoId,
    string PlanAccion,
    DateOnly? FechaCompromiso,
    string? ResponsableEmail
) : IRequest;

public class ActualizarPlanAccionValidator : AbstractValidator<ActualizarPlanAccionCommand>
{
    public ActualizarPlanAccionValidator()
    {
        RuleFor(x => x.HallazgoId).NotEmpty();
        RuleFor(x => x.PlanAccion).NotEmpty().MaximumLength(4000);
    }
}

public class ActualizarPlanAccionHandler : IRequestHandler<ActualizarPlanAccionCommand>
{
    private readonly IHallazgoRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLoggerService _auditLogger;

    public ActualizarPlanAccionHandler(IHallazgoRepository repo, ICurrentUserService currentUser, IAuditLoggerService auditLogger)
    {
        _repo = repo;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
    }

    public async Task Handle(ActualizarPlanAccionCommand request, CancellationToken cancellationToken)
    {
        var hallazgo = await _repo.GetByIdAsync(request.HallazgoId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hallazgo {request.HallazgoId} no encontrado.");

        var antes = new { hallazgo.PlanAccion, hallazgo.FechaCompromiso, hallazgo.ResponsableEmail };
        hallazgo.PlanAccion = request.PlanAccion;
        hallazgo.FechaCompromiso = request.FechaCompromiso;
        hallazgo.ResponsableEmail = request.ResponsableEmail;
        if (hallazgo.Estado == EstadoHallazgo.ABIERTO)
            hallazgo.Estado = EstadoHallazgo.EN_PROCESO;

        await _repo.UpdateAsync(hallazgo, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        await _auditLogger.LogAsync(_currentUser.UserId, _currentUser.Email,
            "ACTUALIZAR", "Hallazgo", hallazgo.Id.ToString(), antes, new { hallazgo.PlanAccion }, ct: cancellationToken);
    }
}

public record CerrarHallazgoCommand(Guid HallazgoId, string Justificacion) : IRequest;

public class CerrarHallazgoHandler : IRequestHandler<CerrarHallazgoCommand>
{
    private readonly IHallazgoRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public CerrarHallazgoHandler(IHallazgoRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task Handle(CerrarHallazgoCommand request, CancellationToken cancellationToken)
    {
        var hallazgo = await _repo.GetByIdAsync(request.HallazgoId, cancellationToken)
            ?? throw new KeyNotFoundException($"Hallazgo {request.HallazgoId} no encontrado.");

        hallazgo.Estado = EstadoHallazgo.CERRADO;
        hallazgo.FechaCierre = DateOnly.FromDateTime(DateTime.UtcNow);
        hallazgo.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(hallazgo, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}
