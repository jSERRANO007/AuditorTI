using AuditorPRO.Application.Common.Models;
using AuditorPRO.Application.Features.Simulaciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditorPRO.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SimulacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SimulacionesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SimulacionListDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetSimulacionesQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SimulacionDetalleDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSimulacionDetalleQuery(id), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "AuditorPRO.Admin,AuditorPRO.Auditor,AuditorPRO.TI.Senior")]
    [ProducesResponseType(typeof(Guid), 201)]
    public async Task<IActionResult> Iniciar([FromBody] IniciarSimulacionCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/cancelar")]
    [Authorize(Roles = "AuditorPRO.Admin,AuditorPRO.Auditor")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new CancelarSimulacionCommand(id), ct);
        return NoContent();
    }
}
