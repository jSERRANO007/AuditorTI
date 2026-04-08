using AuditorPRO.Application.Features.IA;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditorPRO.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IAController : ControllerBase
{
    private readonly IMediator _mediator;

    public IAController(IMediator mediator) => _mediator = mediator;

    [HttpPost("consultar")]
    [ProducesResponseType(typeof(IAResponseDto), 200)]
    public async Task<IActionResult> Consultar([FromBody] ConsultarRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new ConsultarIAQuery(request.Pregunta, request.ContextoAdicional), ct);
        return Ok(result);
    }
}

public record ConsultarRequest(string Pregunta, string? ContextoAdicional = null);
