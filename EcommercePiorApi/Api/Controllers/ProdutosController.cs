using Microsoft.AspNetCore.Mvc;
using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Application.Interfaces;

namespace PiorEcommerceDoMundo.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutosController(IProdutoService service)
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
    public Task<IActionResult> Criar([FromBody] CriarProdutoRequest request)
    {
        return Executar(async () =>
        {
            var produto = await _service.CriarAsync(request);
            return CreatedAtAction(nameof(Buscar), new { id = produto.Id }, produto);
        });
    }

    [HttpPut("{id}")]
    public Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProdutoRequest request)
    {
        return Executar(async () => Ok(await _service.AtualizarAsync(id, request)));
    }

    [HttpDelete("{id}")]
    public Task<IActionResult> Remover(int id)
    {
        return Executar(async () =>
        {
            await _service.RemoverAsync(id);
            return NoContent();
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
    }
}