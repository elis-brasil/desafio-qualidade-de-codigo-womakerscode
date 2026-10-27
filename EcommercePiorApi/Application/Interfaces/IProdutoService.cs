using PiorEcommerceDoMundo.Application.DTOs;

namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface IProdutoService
{
    Task<List<ProdutoResponse>> ListarAsync();
    Task<ProdutoResponse> ObterPorIdAsync(int id);
    Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request);
    Task<ProdutoResponse> AtualizarAsync(int id, AtualizarProdutoRequest request);
    Task RemoverAsync(int id);
}