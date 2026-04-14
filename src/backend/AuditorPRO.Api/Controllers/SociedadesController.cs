using AuditorPRO.Application.Features.Sociedades;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditorPRO.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SociedadesController : ControllerBase
{
    private readonly IMediator _mediator;
    public SociedadesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] bool? soloActivas, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSociedadesQuery(soloActivas), ct));

    [HttpGet("{codigo}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCodigo(string codigo, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSociedadByCodigoQuery(codigo.ToUpperInvariant()), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] CrearSociedadRequest req, CancellationToken ct)
    {
        try
        {
            var dto = await _mediator.Send(new CrearSociedadCommand(req.Codigo, req.Nombre, req.Pais), ct);
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo }, dto);
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarSociedadRequest req, CancellationToken ct)
    {
        try { return Ok(await _mediator.Send(new ActualizarSociedadCommand(id, req.Nombre, req.Pais), ct)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPatch("{id:int}/toggle")]
    [AllowAnonymous]
    public async Task<IActionResult> Toggle(int id, CancellationToken ct)
    {
        try { return Ok(await _mediator.Send(new ToggleSociedadCommand(id), ct)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("cargar-excel")]
    [AllowAnonymous]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult> CargarExcel([FromForm] IFormFile archivo, CancellationToken ct)
    {
        var ms = new MemoryStream();
        await archivo.CopyToAsync(ms, ct);
        ms.Position = 0;
        var resultado = await _mediator.Send(new CargarSociedadesCommand(ms), ct);
        return Ok(resultado);
    }

    [HttpGet("plantilla")]
    [AllowAnonymous]
    public IActionResult Plantilla()
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Sociedades");
        string[] headers = ["CODIGO_SOCIEDAD*", "NOMBRE_SOCIEDAD*", "PAIS", "ESTADO (Activa/Inactiva)"];
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
            ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
        }
        ws.Column(1).Width = 18; ws.Column(2).Width = 40; ws.Column(3).Width = 20; ws.Column(4).Width = 22;
        // Ejemplo
        ws.Cell(2, 1).Value = "XX01"; ws.Cell(2, 2).Value = "Nueva Sociedad S.A.";
        ws.Cell(2, 3).Value = "Costa Rica"; ws.Cell(2, 4).Value = "Activa";

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "plantilla_sociedades.xlsx");
    }
}

public record CrearSociedadRequest(string Codigo, string Nombre, string? Pais);
public record ActualizarSociedadRequest(string Nombre, string? Pais);
