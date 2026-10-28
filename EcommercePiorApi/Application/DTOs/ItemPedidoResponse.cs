namespace PiorEcommerceDoMundo.Application.DTOs;

public class ItemPedidoResponse
{
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
