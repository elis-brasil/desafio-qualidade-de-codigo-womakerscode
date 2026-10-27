namespace PiorEcommerceDoMundo.Domain.Strategies.Descontos;

/// <summary>
/// Aplica desconto de 10% para compras com subtotal superior a R$ 500,00.
/// Elimina os números mágicos 500 e 0.10 (Problema #6).
/// </summary>
public class DescontoValorAltoStrategy : IDescontoStrategy
{
    public const decimal LimiteParaDesconto = 500.00m;
    public const decimal PercentualDesconto = 0.10m; // 10%

    public bool SeAplica(decimal subtotal, string clienteEmail)
    {
        return subtotal > LimiteParaDesconto;
    }

    public decimal CalcularDesconto(decimal subtotal)
    {
        return subtotal * PercentualDesconto;
    }
}
