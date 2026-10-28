using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface IPedidoRepository
{
    Task<List<Pedido>> ListarAsync();
    Task<Pedido> ObterPorIdAsync(int id);
    Task AdicionarAsync(Pedido pedido);
    Task SalvarAsync();
}
