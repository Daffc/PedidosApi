using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("pedidos")]
public sealed class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(
        IPedidoService pedidoService,
        ILogger<PedidosController> logger
    )
    {
        _pedidoService = pedidoService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreatePedidoRequest request
    )
    {
        _logger.LogInformation($"Criando pedido para cliente {request.ClienteNome}");

        var response = await _pedidoService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response
        );
    }

    [HttpGet("{id:guid}", Name = "GetById")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation($"Obtendo pedido {id}");

        var response = await _pedidoService.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PedidoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPagedAsync(
        [FromQuery] GetPedidosRequest request
    )
    {
        _logger.LogInformation($"Listando pedidos. Status: {request.StatusPedido}, Página: {request.Page}, Tamanho: {request.PageSize}");

        var response = await _pedidoService.GetPagedAsync(request);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id
    )
    {
        _logger.LogInformation($"Cancelando pedido {id}");

        await _pedidoService.CancelAsync(id);

        return NoContent();
    }
}