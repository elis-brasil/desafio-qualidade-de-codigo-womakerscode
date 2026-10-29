
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Infrastructure.Data;
using PiorEcommerceDoMundo.Services;

namespace PiorEcommerceDoMundo.Controllers;

[ApiController]
[Route("api/logistica")]
public class LogisticaController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogisticaService _logisticaService;

    public LogisticaController(
        AppDbContext db,
        ILogisticaService logisticaService)
    {
        _db = db;
        _logisticaService = logisticaService;
    }

    [HttpPost("reenviar/{pedidoId:int}")]
    public async Task<IActionResult> ReenviarLogistica(
        int pedidoId,
        CancellationToken cancellationToken)
    {
        var pedido = await _db.Pedidos
            .FirstOrDefaultAsync(
                p => p.Id == pedidoId,
                cancellationToken);

        if (pedido == null)
        {
            return NotFound(new
            {
                mensagem = "Pedido não encontrado."
            });
        }

        await _logisticaService.ProcessarEnvioAsync(
            pedido, cancellationToken);

        return Ok(pedido);
    }
}
