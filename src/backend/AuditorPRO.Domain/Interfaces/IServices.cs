namespace AuditorPRO.Domain.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string Email { get; }
    string DisplayName { get; }
    IEnumerable<string> Roles { get; }
    bool IsInRole(string role);
}

public interface IAuditLoggerService
{
    Task LogAsync(string userId, string email, string accion, string recurso, string? recursoId = null,
        object? datosAntes = null, object? datosDespues = null, bool exitoso = true, string? errorDetalle = null,
        CancellationToken ct = default);
    Task LogSecurityEventAsync(string userId, string evento, string path, CancellationToken ct = default);
}

public interface IAzureOpenAIService
{
    Task<string> AnalizarControlAsync(string contexto, string descripcionControl, string resultado, CancellationToken ct = default);
    Task<string> GenerarRecomendacionAsync(string hallazgo, string dominio, CancellationToken ct = default);
    Task<string> ConsultarAsync(string pregunta, string contextoOrganizacional, CancellationToken ct = default);
}

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, string container, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string blobUrl, CancellationToken ct = default);
    Task DeleteAsync(string blobUrl, CancellationToken ct = default);
    Task<string> GenerateSasTokenAsync(string blobUrl, TimeSpan expiry, CancellationToken ct = default);
}

public interface IDocumentGeneratorService
{
    Task<byte[]> GenerateWordReportAsync(Guid simulacionId, CancellationToken ct = default);
    Task<byte[]> GeneratePptSummaryAsync(Guid simulacionId, CancellationToken ct = default);
    Task<byte[]> GenerateHallazgosExcelAsync(IEnumerable<Guid> hallazgoIds, CancellationToken ct = default);
}
