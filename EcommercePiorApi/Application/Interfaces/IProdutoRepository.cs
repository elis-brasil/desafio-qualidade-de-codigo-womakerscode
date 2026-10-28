using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface IProdutoRepository
{
    Task<List<Produto>> ListarAsync();
    Task<Produto> ObterPorIdAsync(int id);
    Task AdicionarAsync(Produto produto);
    void Remover(Produto produto);
    Task SalvarAsync();
}