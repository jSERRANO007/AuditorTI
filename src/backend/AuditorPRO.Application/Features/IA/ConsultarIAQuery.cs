using AuditorPRO.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AuditorPRO.Application.Features.IA;

public record ConsultarIAQuery(string Pregunta, string? ContextoAdicional = null) : IRequest<IAResponseDto>;

public class ConsultarIAValidator : AbstractValidator<ConsultarIAQuery>
{
    public ConsultarIAValidator()
    {
        RuleFor(x => x.Pregunta).NotEmpty().MaximumLength(2000);
    }
}

public class IAResponseDto
{
    public string Respuesta { get; set; } = string.Empty;
    public string? FuentesConsultadas { get; set; }
    public DateTime GeneradoAt { get; set; } = DateTime.UtcNow;
}

public class ConsultarIAHandler : IRequestHandler<ConsultarIAQuery, IAResponseDto>
{
    private readonly IAzureOpenAIService _iaService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLoggerService _auditLogger;

    private const string ContextoOrganizacional = """
        Eres el Agente Auditor Preventivo de AuditorPRO TI para ILG Logistics.
        Tu rol es asistir a auditores internos y administradores de TI a identificar debilidades de control,
        evaluar riesgos de segregación de funciones, revisar políticas de seguridad y sugerir planes de acción.
        Baséate en marcos normativos: ISO 27001, COBIT 2019, COSO, SOX, NIST CSF.
        Responde siempre en español, con base en evidencia y fundamento normativo.
        """;

    public ConsultarIAHandler(IAzureOpenAIService iaService, ICurrentUserService currentUser, IAuditLoggerService auditLogger)
    {
        _iaService = iaService;
        _currentUser = currentUser;
        _auditLogger = auditLogger;
    }

    public async Task<IAResponseDto> Handle(ConsultarIAQuery request, CancellationToken cancellationToken)
    {
        var contexto = request.ContextoAdicional != null
            ? $"{ContextoOrganizacional}\n\nContexto adicional:\n{request.ContextoAdicional}"
            : ContextoOrganizacional;

        var respuesta = await _iaService.ConsultarAsync(request.Pregunta, contexto, cancellationToken);

        await _auditLogger.LogAsync(_currentUser.UserId, _currentUser.Email,
            "CONSULTA_IA", "AgentIA", null, ct: cancellationToken);

        return new IAResponseDto { Respuesta = respuesta };
    }
}
