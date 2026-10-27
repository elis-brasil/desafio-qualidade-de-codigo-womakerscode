using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Infrastructure.Data;

namespace PiorEcommerceDoMundo.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _db;

    public PedidoRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Pedido>> ListarAsync()
    {
        return _db.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .ToListAsync();
    }

    public Task<Pedido> ObterPorIdAsync(int id)
    {
        return _db.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _db.Pedidos.AddAsync(pedido);
    }

    public Task SalvarAsync()
    {
        return _db.SaveChangesAsync();
    }
}
