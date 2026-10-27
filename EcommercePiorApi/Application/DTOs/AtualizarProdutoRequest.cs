namespace PiorEcommerceDoMundo.Application.DTOs;

public class AtualizarProdutoRequest
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Preco { get; set; }
    public bool Ativo { get; set; }
}