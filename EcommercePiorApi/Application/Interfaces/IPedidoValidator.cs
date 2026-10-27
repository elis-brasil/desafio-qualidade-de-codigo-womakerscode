using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface IPedidoValidator
{
    void ValidarRequisicao(PedidoRequest request);
    void ValidarProdutosEEstoque(List<ItemRequest> itens, IDictionary<int, Produto> produtosCadastrados);
}
