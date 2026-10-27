namespace PiorEcommerceDoMundo.Domain.Entities;

public class Pedido
{
    public int Id { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public DateTime DataPedido { get; set; }

    // status controlado por string solta, sem enum: "Pendente", "Pago", "Falhou", "Enviado"
    public string Status { get; set; }
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; }
    public string CodigoRastreio { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
}