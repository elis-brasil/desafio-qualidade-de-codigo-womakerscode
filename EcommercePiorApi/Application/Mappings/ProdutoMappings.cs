using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Mappings;

public static class ProdutoMappings
{
    public static ProdutoResponse ToResponse(this Produto produto)
    {
        return new ProdutoResponse
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            QuantidadeEstoque = produto.QuantidadeEstoque,
            Ativo = produto.Ativo
        };
    }

    public static EstoqueResponse ToEstoqueResponse(this Produto produto)
    {
        return new EstoqueResponse
        {
            ProdutoId = produto.Id,
            Produto = produto.Nome,
            Quantidade = produto.QuantidadeEstoque
        };
    }
}