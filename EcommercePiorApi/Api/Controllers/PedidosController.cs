using Microsoft.AspNetCore.Mvc;
using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Application.Interfaces;

namespace PiorEcommerceDoMundo.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;

    public PedidosController(IPedidoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _service.ListarAsync());
    }

    [HttpGet("{id}")]
    public Task<IActionResult> Buscar(int id)
    {
        return Executar(async () => Ok(await _service.ObterPorIdAsync(id)));
    }

    [HttpPost]
    public Task<IActionResult> Criar([FromBody] PedidoRequest request)
    {
        return Executar(async () =>
        {
            var pedido = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(Buscar), new { id = pedido.Id }, pedido);
        });
    }

    // TODO: remover quando existir o middleware global de erros (problema 12, Dev 5)
    private async Task<IActionResult> Executar(Func<Task<IActionResult>> acao)
    {
        try
        {
            return await acao();
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
