using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Mappings;

public static class PedidoMappings
{
    public static PedidoResponse ToResponse(this Pedido pedido)
    {
        return new PedidoResponse
        {
            Id = pedido.Id,
            ClienteNome = pedido.ClienteNome,
            ClienteEmail = pedido.ClienteEmail,
            DataPedido = pedido.DataPedido,
            Status = pedido.Status.ToString(),
            ValorDesconto = pedido.ValorDesconto,
            ValorTotal = pedido.ValorTotal,
            Subtotal = pedido.ValorTotal + pedido.ValorDesconto,
            FormaPagamento = pedido.FormaPagamento,
            CodigoRastreio = pedido.CodigoRastreio,
            Itens = pedido.Itens.Select(i => i.ToResponse()).ToList()
        };
    }

    public static ItemPedidoResponse ToResponse(this ItemPedido item)
    {
        return new ItemPedidoResponse
        {
            ProdutoId = item.ProdutoId,
            NomeProduto = item.Produto?.Nome,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario
        };
    }
}
