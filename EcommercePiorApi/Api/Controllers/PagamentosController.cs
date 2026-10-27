using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Infrastructure.Data;

namespace PiorEcommerceDoMundo.Api.Controllers;

[ApiController]
[Route("api/pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPagamentoService _pagamentoService;
    private readonly IEstoqueService _estoqueService;

    public PagamentosController(AppDbContext db, IPagamentoService pagamentoService, IEstoqueService estoqueService)
    {
        _db = db;
        _pagamentoService = pagamentoService;
        _estoqueService = estoqueService;
    }

    [HttpPost("reprocessar/{pedidoId}")]
    public async Task<IActionResult> ReprocessarPagamento(int pedidoId)
    {
        var pedido = await _db.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null) return NotFound();

        if (pedido.Status != "Falhou") return BadRequest("Somente pedidos com pagamento falho podem ser reprocessados");

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in pedido.Itens)
            {
                await _estoqueService.DebitarAsync(item.ProdutoId, item.Quantidade);
            }

            var pagamentoAprovado = await _pagamentoService.ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);

            if (!pagamentoAprovado)
            {
                await transaction.RollbackAsync();
                _db.ChangeTracker.Clear();
                return BadRequest("pagamento recusado novamente");
            }

            pedido.Status = "Pago";
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            await transaction.RollbackAsync();
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }
}