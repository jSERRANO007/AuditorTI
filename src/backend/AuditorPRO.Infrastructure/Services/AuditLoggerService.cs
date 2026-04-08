using System.Text.Json;
using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Enums;
using AuditorPRO.Domain.Interfaces;

namespace AuditorPRO.Infrastructure.Services;

public class AuditLoggerService : IAuditLoggerService
{
    private readonly IBitacoraRepository _bitacoraRepo;

    public AuditLoggerService(IBitacoraRepository bitacoraRepo) => _bitacoraRepo = bitacoraRepo;

    public async Task LogAsync(string userId, string email, string accion, string recurso,
        string? recursoId = null, object? datosAntes = null, object? datosDespues = null,
        bool exitoso = true, string? errorDetalle = null, CancellationToken ct = default)
    {
        if (!Enum.TryParse<AccionBitacora>(accion, true, out var accionEnum))
            accionEnum = AccionBitacora.ACTUALIZAR;

        var evento = new BitacoraEvento
        {
            UsuarioId = userId,
            UsuarioEmail = email,
            Accion = accionEnum,
            Recurso = recurso,
            RecursoId = recursoId,
            DatosAntes = datosAntes != null ? JsonSerializer.Serialize(datosAntes) : null,
            DatosDespues = datosDespues != null ? JsonSerializer.Serialize(datosDespues) : null,
            Exitoso = exitoso,
            ErrorDetalle = errorDetalle
        };

        await _bitacoraRepo.LogAsync(evento, ct);
    }

    public async Task LogSecurityEventAsync(string userId, string evento, string path, CancellationToken ct = default)
    {
        var bitacoraEvento = new BitacoraEvento
        {
            UsuarioId = userId,
            Accion = AccionBitacora.LOGIN,
            Recurso = path,
            Descripcion = evento,
            Exitoso = false
        };

        await _bitacoraRepo.LogAsync(bitacoraEvento, ct);
    }
}
