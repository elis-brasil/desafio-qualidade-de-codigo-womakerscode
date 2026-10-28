using Microsoft.AspNetCore.Mvc;
using PiorEcommerceDoMundo.Application.Interfaces;

namespace PiorEcommerceDoMundo.Api.Controllers;

[ApiController]
[Route("api/pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PagamentosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost("reprocessar/{pedidoId}")]
    public async Task<IActionResult> ReprocessarPagamento(int pedidoId)
    {
        try
        {
            var pedido = await _pedidoService.ReprocessarPagamentoAsync(pedidoId);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}