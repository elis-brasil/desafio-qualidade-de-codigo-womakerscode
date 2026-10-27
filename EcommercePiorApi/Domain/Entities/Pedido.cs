using PiorEcommerceDoMundo.Domain.Enums;

namespace PiorEcommerceDoMundo.Domain.Entities;

public class Pedido
{
    public int Id { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public DateTime DataPedido { get; set; }

    // Status gerenciado por enum fortemente tipado (Problema 13)
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    public decimal ValorTotal { get; set; }
    public decimal ValorDesconto { get; set; }
    public string FormaPagamento { get; set; }
    public string CodigoRastreio { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
}
