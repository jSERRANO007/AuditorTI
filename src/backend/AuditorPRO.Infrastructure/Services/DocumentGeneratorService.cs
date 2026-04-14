using AuditorPRO.Domain.Entities;
using AuditorPRO.Domain.Enums;
using AuditorPRO.Domain.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using P = DocumentFormat.OpenXml.Presentation;
using D = DocumentFormat.OpenXml.Drawing;

namespace AuditorPRO.Infrastructure.Services;

public class DocumentGeneratorService : IDocumentGeneratorService
{
    private readonly ISimulacionRepository _simulacionRepo;
    private readonly IHallazgoRepository _hallazgoRepo;
    private readonly ILogger<DocumentGeneratorService> _logger;

    private static readonly Dictionary<string, string> TipoHallazgoDescripcion = new()
    {
        ["R01_SOD"] = "Segregación de Funciones (SoD)",
        ["R02_ACCESO_EX_EMPLEADO"] = "Accesos de ex-empleados activos",
        ["R03_ROL_NO_AUTORIZADO_MATRIZ"] = "Roles no autorizados según Matriz",
        ["R04_USUARIO_SIN_ENTRA_ID"] = "Usuarios SAP sin cuenta Entra ID",
        ["R05_ROL_SIN_TRANSACCIONES"] = "Roles asignados sin uso de transacciones",
        ["SOD"] = "Segregación de Funciones (SoD)",
        ["ACCESO_EX_EMPLEADO"] = "Accesos de ex-empleados activos",
        ["ROL_NO_AUTORIZADO_MATRIZ"] = "Roles no autorizados según Matriz",
        ["USUARIO_SIN_ENTRA_ID"] = "Usuarios SAP sin cuenta Entra ID",
        ["ROL_SIN_TRANSACCIONES"] = "Roles asignados sin uso de transacciones",
    };

    public DocumentGeneratorService(
        ISimulacionRepository simulacionRepo,
        IHallazgoRepository hallazgoRepo,
        ILogger<DocumentGeneratorService> logger)
    {
        _simulacionRepo = simulacionRepo;
        _hallazgoRepo = hallazgoRepo;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // WORD
    // ─────────────────────────────────────────────────────────────────────────
    public async Task<byte[]> GenerateWordReportAsync(Guid simulacionId, CancellationToken ct = default)
    {
        var simulacion = await _simulacionRepo.GetWithResultadosAsync(simulacionId, ct)
            ?? throw new KeyNotFoundException($"Simulación {simulacionId} no encontrada.");

        var hallazgos = (await _hallazgoRepo.GetBySimulacionAsync(simulacionId, ct)).ToList();

        using var ms = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // ── Estilos ────────────────────────────────────────────────────
            var stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
            stylePart.Styles = BuildWordStyles();

            // ── Portada ────────────────────────────────────────────────────
            AppendParagraph(body, "INFORME DE AUDITORÍA DE ACCESOS SAP", "Title");
            AppendParagraph(body, simulacion.Nombre, "Subtitle");
            AppendParagraph(body, $"Período: {simulacion.PeriodoInicio:dd/MM/yyyy} — {simulacion.PeriodoFin:dd/MM/yyyy}");
            AppendParagraph(body, $"Completado: {simulacion.CompletadaAt?.ToString("dd/MM/yyyy HH:mm") ?? "En proceso"}");
            AppendParagraph(body, "");

            // ── Resumen ejecutivo ──────────────────────────────────────────
            AppendParagraph(body, "1. Resumen Ejecutivo", "Heading1");

            var totalH = hallazgos.Count;
            var criticos = hallazgos.Count(h => h.Criticidad == Criticidad.CRITICA);
            var medios   = hallazgos.Count(h => h.Criticidad == Criticidad.MEDIA);
            var bajos    = hallazgos.Count(h => h.Criticidad == Criticidad.BAJA);

            AppendParagraph(body, $"La simulación de auditoría identificó un total de {totalH} hallazgos de control de acceso SAP:");
            AppendParagraph(body, $"  • Críticos: {criticos}");
            AppendParagraph(body, $"  • Medios: {medios}");
            AppendParagraph(body, $"  • Bajos: {bajos}");
            AppendParagraph(body, "");

            if (simulacion.ScoreMadurez.HasValue)
                AppendParagraph(body, $"Score de Madurez: {simulacion.ScoreMadurez:F2} / 10.00");
            if (simulacion.PorcentajeCumplimiento.HasValue)
                AppendParagraph(body, $"Porcentaje de Cumplimiento: {simulacion.PorcentajeCumplimiento:F1}%");

            AppendParagraph(body, "");

            // ── Hallazgos por regla ────────────────────────────────────────
            AppendParagraph(body, "2. Hallazgos por Regla de Control", "Heading1");

            var porRegla = hallazgos
                .GroupBy(h => h.TipoHallazgo ?? "SIN_CLASIFICAR")
                .OrderByDescending(g => g.Count())
                .ToList();

            foreach (var grupo in porRegla)
            {
                var descripcion = TipoHallazgoDescripcion.GetValueOrDefault(grupo.Key, grupo.Key);
                AppendParagraph(body, $"Regla: {descripcion}", "Heading2");
                AppendParagraph(body, $"Total: {grupo.Count()} hallazgos   |   "
                    + $"Críticos: {grupo.Count(h => h.Criticidad == Criticidad.CRITICA)}   "
                    + $"Medios: {grupo.Count(h => h.Criticidad == Criticidad.MEDIA)}   "
                    + $"Bajos: {grupo.Count(h => h.Criticidad == Criticidad.BAJA)}");
                AppendParagraph(body, "");

                // Tabla de hallazgos
                var tabla = body.AppendChild(new Table());
                tabla.AppendChild(BuildTableProperties());

                // Encabezado
                var header = tabla.AppendChild(new TableRow());
                AddHeaderCell(header, "Usuario SAP");
                AddHeaderCell(header, "Cédula");
                AddHeaderCell(header, "Rol Afectado");
                AddHeaderCell(header, "Criticidad");
                AddHeaderCell(header, "Descripción");

                // Filas (máx 200 por regla para no sobrecargar el doc)
                foreach (var h in grupo.OrderByDescending(h => h.Criticidad).Take(200))
                {
                    var row = tabla.AppendChild(new TableRow());
                    AddCell(row, h.UsuarioSAP ?? "—");
                    AddCell(row, h.Cedula ?? "—");
                    AddCell(row, h.RolAfectado ?? "—");
                    AddCell(row, h.Criticidad.ToString());
                    AddCell(row, TruncateDescription(h.Descripcion, 120));
                }

                if (grupo.Count() > 200)
                    AppendParagraph(body, $"  * Se muestran 200 de {grupo.Count()} hallazgos. Para ver el listado completo, exporte a Excel.");

                AppendParagraph(body, "");
            }

            // ── Recomendaciones ────────────────────────────────────────────
            AppendParagraph(body, "3. Recomendaciones", "Heading1");
            AppendParagraph(body, "Con base en los hallazgos identificados, se recomienda:");
            AppendParagraph(body, "  1. Revisar y revocar inmediatamente los accesos críticos identificados en la matriz de puestos.");
            AppendParagraph(body, "  2. Completar el proceso de baja de accesos SAP para ex-empleados detectados.");
            AppendParagraph(body, "  3. Actualizar la Matriz de Puestos para reflejar los roles efectivamente requeridos por cada puesto.");
            AppendParagraph(body, "  4. Implementar un proceso de revisión periódica de accesos SAP (mínimo trimestral).");
            AppendParagraph(body, "  5. Garantizar que todos los usuarios SAP activos tengan cuenta Entra ID corporativa vigente.");

            doc.Save();
        }

        return ms.ToArray();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PPT
    // ─────────────────────────────────────────────────────────────────────────
    public async Task<byte[]> GeneratePptSummaryAsync(Guid simulacionId, CancellationToken ct = default)
    {
        var simulacion = await _simulacionRepo.GetWithResultadosAsync(simulacionId, ct)
            ?? throw new KeyNotFoundException($"Simulación {simulacionId} no encontrada.");

        var hallazgos = (await _hallazgoRepo.GetBySimulacionAsync(simulacionId, ct)).ToList();

        using var ms = new MemoryStream();
        using (var ppt = PresentationDocument.Create(ms, PresentationDocumentType.Presentation))
        {
            var presentationPart = ppt.AddPresentationPart();
            presentationPart.Presentation = new P.Presentation();

            var slideIdList   = new P.SlideIdList();
            var slideSize     = new P.SlideSize { Cx = 9144000, Cy = 5143500, Type = P.SlideSizeValues.Screen16x9 };
            var notesSz       = new P.NotesSize { Cx = 6858000, Cy = 9144000 };
            presentationPart.Presentation.Append(slideIdList, slideSize, notesSz);

            uint slideId = 256;

            // Slide 1 — Portada
            AddSlide(presentationPart, slideIdList, ref slideId,
                title: "Informe de Auditoría SAP",
                lines: new[]
                {
                    simulacion.Nombre,
                    $"Período: {simulacion.PeriodoInicio:dd/MM/yyyy} — {simulacion.PeriodoFin:dd/MM/yyyy}",
                    $"Generado: {DateTime.Now:dd/MM/yyyy}",
                });

            // Slide 2 — Dashboard de riesgo
            var totalH  = hallazgos.Count;
            var criticos = hallazgos.Count(h => h.Criticidad == Criticidad.CRITICA);
            var medios   = hallazgos.Count(h => h.Criticidad == Criticidad.MEDIA);
            var bajos    = hallazgos.Count(h => h.Criticidad == Criticidad.BAJA);

            AddSlide(presentationPart, slideIdList, ref slideId,
                title: "Dashboard de Riesgo",
                lines: new[]
                {
                    $"Total de hallazgos identificados: {totalH}",
                    "",
                    $"🔴  Críticos: {criticos}   ({Pct(criticos, totalH)}%)",
                    $"🟡  Medios:   {medios}   ({Pct(medios, totalH)}%)",
                    $"🟢  Bajos:    {bajos}   ({Pct(bajos, totalH)}%)",
                    "",
                    simulacion.ScoreMadurez.HasValue
                        ? $"Score de Madurez: {simulacion.ScoreMadurez:F2} / 10"
                        : "Score de Madurez: calculando...",
                });

            // Slides por regla
            var porRegla = hallazgos
                .GroupBy(h => h.TipoHallazgo ?? "SIN_CLASIFICAR")
                .OrderByDescending(g => g.Count())
                .ToList();

            foreach (var grupo in porRegla)
            {
                var descripcion = TipoHallazgoDescripcion.GetValueOrDefault(grupo.Key, grupo.Key);
                var top5 = grupo.OrderByDescending(h => h.Criticidad).Take(5).ToList();
                var lines = new List<string>
                {
                    $"Total: {grupo.Count()}   |   Críticos: {grupo.Count(h => h.Criticidad == Criticidad.CRITICA)}   Medios: {grupo.Count(h => h.Criticidad == Criticidad.MEDIA)}   Bajos: {grupo.Count(h => h.Criticidad == Criticidad.BAJA)}",
                    "",
                    "Ejemplos representativos:",
                };
                foreach (var h in top5)
                    lines.Add($"• {h.UsuarioSAP ?? "?"} / {h.Cedula ?? "?"} — {h.RolAfectado ?? "?"} [{h.Criticidad}]");

                AddSlide(presentationPart, slideIdList, ref slideId,
                    title: descripcion,
                    lines: lines.ToArray());
            }

            // Slide final — Recomendaciones
            AddSlide(presentationPart, slideIdList, ref slideId,
                title: "Recomendaciones Principales",
                lines: new[]
                {
                    "1. Revocar accesos críticos identificados en la Matriz de Puestos.",
                    "2. Completar baja de accesos SAP para ex-empleados.",
                    "3. Actualizar la Matriz de Puestos con roles reales requeridos.",
                    "4. Implementar revisión periódica trimestral de accesos SAP.",
                    "5. Asegurar cuenta Entra ID para todos los usuarios SAP activos.",
                });

            presentationPart.Presentation.Save();
        }

        return ms.ToArray();
    }

    public async Task<byte[]> GenerateHallazgosExcelAsync(IEnumerable<Guid> hallazgoIds, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Word helpers
    // ─────────────────────────────────────────────────────────────────────────
    private static void AppendParagraph(Body body, string text, string? style = null)
    {
        var para = body.AppendChild(new Paragraph());
        if (style != null)
        {
            var props = para.AppendChild(new ParagraphProperties());
            props.AppendChild(new ParagraphStyleId { Val = style });
        }
        var run = para.AppendChild(new Run());
        run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
    }

    private static TableProperties BuildTableProperties()
    {
        return new TableProperties(
            new TableBorders(
                new TopBorder    { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                new RightBorder  { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder   { Val = BorderValues.Single, Size = 4 }
            )
        );
    }

    private static void AddHeaderCell(TableRow row, string text)
    {
        var cell = row.AppendChild(new TableCell());
        var props = cell.AppendChild(new TableCellProperties());
        props.AppendChild(new Shading { Fill = "1F3864", Color = "auto", Val = ShadingPatternValues.Clear });
        var para = cell.AppendChild(new Paragraph());
        var run = para.AppendChild(new Run());
        run.AppendChild(new RunProperties(new Bold(), new Color { Val = "FFFFFF" }));
        run.AppendChild(new Text(text));
    }

    private static void AddCell(TableRow row, string text)
    {
        var cell = row.AppendChild(new TableCell());
        var para = cell.AppendChild(new Paragraph());
        var run = para.AppendChild(new Run());
        run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
    }

    private static Styles BuildWordStyles()
    {
        var styles = new Styles();
        styles.AppendChild(new DocDefaults(
            new RunPropertiesDefault(new RunPropertiesBaseStyle(
                new RunFonts { Ascii = "Calibri" },
                new FontSize { Val = "22" }
            ))
        ));
        styles.AppendChild(MakeStyle("Title",    "Title",    32, true, "1F3864"));
        styles.AppendChild(MakeStyle("Subtitle", "Subtitle", 24, false, "4472C4"));
        styles.AppendChild(MakeStyle("Heading1", "Heading 1", 20, true, "1F3864"));
        styles.AppendChild(MakeStyle("Heading2", "Heading 2", 16, true, "2E75B6"));
        return styles;
    }

    private static Style MakeStyle(string id, string name, int halfPts, bool bold, string hexColor)
    {
        var style = new Style { Type = StyleValues.Paragraph, StyleId = id };
        style.Append(new StyleName { Val = name });
        style.Append(new StyleRunProperties(
            new Bold { Val = bold },
            new Color { Val = hexColor },
            new FontSize { Val = halfPts.ToString() }
        ));
        return style;
    }

    private static string TruncateDescription(string text, int maxLen)
        => text.Length <= maxLen ? text : text[..maxLen] + "…";

    // ─────────────────────────────────────────────────────────────────────────
    // PPT helpers
    // ─────────────────────────────────────────────────────────────────────────
    private static void AddSlide(PresentationPart presentationPart, P.SlideIdList slideIdList, ref uint slideId, string title, string[] lines)
    {
        var slidePart = presentationPart.AddNewPart<SlidePart>();
        var slide     = new P.Slide(new P.CommonSlideData(new P.ShapeTree()));
        var shapeTree = slide.CommonSlideData!.ShapeTree!;

        // Title shape
        shapeTree.AppendChild(BuildPptTextShape(
            id: 1,
            name: "Title",
            x: 457200, y: 274638, cx: 8229600, cy: 1143000,
            text: title,
            fontSize: 3200,
            bold: true,
            hexColor: "1F3864"));

        // Content shape
        var contentText = string.Join("\n", lines);
        shapeTree.AppendChild(BuildPptTextShape(
            id: 2,
            name: "Content",
            x: 457200, y: 1600200, cx: 8229600, cy: 3375000,
            text: contentText,
            fontSize: 1800,
            bold: false,
            hexColor: "000000"));

        slide.Save(slidePart);

        var sid = new P.SlideId { Id = slideId++, RelationshipId = presentationPart.GetIdOfPart(slidePart) };
        slideIdList.AppendChild(sid);
    }

    private static P.Shape BuildPptTextShape(uint id, string name, long x, long y, long cx, long cy,
        string text, int fontSize, bool bold, string hexColor)
    {
        var shape = new P.Shape();

        shape.AppendChild(new P.NonVisualShapeProperties(
            new P.NonVisualDrawingProperties { Id = id, Name = name },
            new P.NonVisualShapeDrawingProperties(new D.ShapeLocks { NoGrouping = true }),
            new P.ApplicationNonVisualDrawingProperties(new P.PlaceholderShape())));

        shape.AppendChild(new P.ShapeProperties(
            new D.Transform2D(
                new D.Offset { X = x, Y = y },
                new D.Extents { Cx = cx, Cy = cy }),
            new D.PresetGeometry { Preset = D.ShapeTypeValues.Rectangle }));

        var txBody = new P.TextBody(
            new D.BodyProperties { Wrap = D.TextWrappingValues.Square },
            new D.ListStyle());

        foreach (var line in text.Split('\n'))
        {
            var para = new D.Paragraph();
            var run  = new D.Run();
            run.AppendChild(new D.RunProperties
            {
                Language  = "es-CR",
                FontSize  = fontSize,
                Bold      = bold,
                Dirty     = false,
            });
            run.AppendChild(new D.SolidFill(new D.RgbColorModelHex { Val = hexColor }));
            run.AppendChild(new D.Text(line));
            para.AppendChild(run);
            txBody.AppendChild(para);
        }

        shape.AppendChild(txBody);
        return shape;
    }

    private static int Pct(int value, int total)
        => total == 0 ? 0 : (int)Math.Round((double)value / total * 100);
}
