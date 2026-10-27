using PiorEcommerceDoMundo.Application.DTOs;

namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface IEstoqueService
{
    Task<List<EstoqueResponse>> ListarAsync();
    Task<EstoqueResponse> ReabastecerAsync(int produtoId, int quantidade);
    Task DebitarAsync(int produtoId, int quantidade);
    Task EstornarAsync(int produtoId, int quantidade);
}