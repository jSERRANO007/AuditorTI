using AuditorPRO.Domain.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;

namespace AuditorPRO.Infrastructure.Services;

public class DocumentGeneratorService : IDocumentGeneratorService
{
    private readonly ISimulacionRepository _simulacionRepo;
    private readonly IHallazgoRepository _hallazgoRepo;
    private readonly ILogger<DocumentGeneratorService> _logger;

    public DocumentGeneratorService(
        ISimulacionRepository simulacionRepo,
        IHallazgoRepository hallazgoRepo,
        ILogger<DocumentGeneratorService> logger)
    {
        _simulacionRepo = simulacionRepo;
        _hallazgoRepo = hallazgoRepo;
        _logger = logger;
    }

    public async Task<byte[]> GenerateWordReportAsync(Guid simulacionId, CancellationToken ct = default)
    {
        var simulacion = await _simulacionRepo.GetWithResultadosAsync(simulacionId, ct)
            ?? throw new KeyNotFoundException($"Simulación {simulacionId} no encontrada.");

        using var ms = new MemoryStream();
        using var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document);

        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());

        // Title
        AppendParagraph(body, $"Informe de Auditoría: {simulacion.Nombre}", "Heading1");
        AppendParagraph(body, $"Fecha: {simulacion.CompletadaAt?.ToString("dd/MM/yyyy") ?? "En proceso"}");
        AppendParagraph(body, $"Score de Madurez: {simulacion.ScoreMadurez:F2} / 10.00");
        AppendParagraph(body, $"Cumplimiento: {simulacion.PorcentajeCumplimiento:F1}%");

        AppendParagraph(body, "Resumen Ejecutivo", "Heading2");
        AppendParagraph(body, $"Total controles evaluados: {simulacion.TotalControles}");
        AppendParagraph(body, $"Controles VERDE: {simulacion.ControlesVerde}");
        AppendParagraph(body, $"Controles AMARILLO: {simulacion.ControlesAmarillo}");
        AppendParagraph(body, $"Controles ROJO: {simulacion.ControlesRojo}");

        AppendParagraph(body, "Detalle de Resultados", "Heading2");
        foreach (var resultado in simulacion.Resultados.OrderBy(r => r.Semaforo))
        {
            AppendParagraph(body, $"[{resultado.Semaforo}] {resultado.PuntoControl?.Codigo} — {resultado.PuntoControl?.Nombre}", "Heading3");
            if (!string.IsNullOrEmpty(resultado.ResultadoDetalle))
                AppendParagraph(body, resultado.ResultadoDetalle);
            if (!string.IsNullOrEmpty(resultado.Recomendacion))
                AppendParagraph(body, $"Recomendación: {resultado.Recomendacion}");
        }

        doc.Save();
        return ms.ToArray();
    }

    public async Task<byte[]> GeneratePptSummaryAsync(Guid simulacionId, CancellationToken ct = default)
    {
        // Simplified — returns a placeholder. Full PPT generation via PresentationML would go here.
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }

    public async Task<byte[]> GenerateHallazgosExcelAsync(IEnumerable<Guid> hallazgoIds, CancellationToken ct = default)
    {
        // Placeholder — would use ClosedXML or DocumentFormat.OpenXml SpreadsheetML
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }

    private static void AppendParagraph(Body body, string text, string? style = null)
    {
        var para = body.AppendChild(new Paragraph());
        if (style != null)
        {
            var props = para.AppendChild(new ParagraphProperties());
            props.AppendChild(new ParagraphStyleId { Val = style });
        }
        var run = para.AppendChild(new Run());
        run.AppendChild(new Text(text));
    }
}
