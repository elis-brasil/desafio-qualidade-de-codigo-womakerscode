namespace PiorEcommerceDoMundo.Application.DTOs;

public class PedidoResponse
{
    public int Id { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public DateTime DataPedido { get; set; }
    public string Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; }
    public string CodigoRastreio { get; set; }
    public List<ItemPedidoResponse> Itens { get; set; } = new();
}
