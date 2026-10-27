using Microsoft.AspNetCore.Mvc;
using PiorEcommerceDoMundo.Application.Interfaces;

namespace PiorEcommerceDoMundo.Api.Controllers;

[ApiController]
[Route("api/estoque")]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueService _service;

    public EstoqueController(IEstoqueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _service.ListarAsync());
    }

    [HttpPost("reabastecer/{produtoId}/{quantidade}")]
    public async Task<IActionResult> Reabastecer(int produtoId, int quantidade)
    {
        // TODO: remover try/catch quando existir o middleware global de erros (problema 12, Dev 5)
        try
        {
            return Ok(await _service.ReabastecerAsync(produtoId, quantidade));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}