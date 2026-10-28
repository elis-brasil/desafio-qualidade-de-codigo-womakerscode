using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Domain.Enums;
using PiorEcommerceDoMundo.Infrastructure.Data;
using PiorEcommerceDoMundo.Infrastructure.Mocks;

namespace PiorEcommerceDoMundo.Api.Controllers;

// Pedidos foram extraídos para PedidosController (Tarefas #1, #5, #6, #8 e #13).
// Os endpoints abaixo permanecem para tratamento pelos responsáveis de Pagamento e Logística.
[ApiController]
public class EcommerceController : ControllerBase
{
    private readonly AppDbContext _db;

    public EcommerceController(AppDbContext db)
    {
        _db = db;
    }

    // ==================== LOGISTICA ====================

    [HttpPost("api/logistica/reenviar/{pedidoId}")]
    public async Task<IActionResult> ReenviarLogistica(int pedidoId)
    {
        var pedido = _db.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        var codigo = await LogisticaMock.EnviarAsync(pedido);
        pedido.CodigoRastreio = codigo;
        pedido.Status = StatusPedido.Enviado;
        await _db.SaveChangesAsync();
        return Ok(pedido);
    }
}
