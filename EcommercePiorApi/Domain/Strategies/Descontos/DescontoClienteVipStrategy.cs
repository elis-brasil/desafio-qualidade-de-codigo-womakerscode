namespace PiorEcommerceDoMundo.Domain.Strategies.Descontos;

/// <summary>
/// Aplica desconto de 20% para clientes VIP (identificados pelo e-mail contendo "vip").
/// Elimina os números mágicos e strings literais soltas (Problema #6).
/// </summary>
public class DescontoClienteVipStrategy : IDescontoStrategy
{
    public const string SufixoIdentificadorVip = "vip";
    public const decimal PercentualDesconto = 0.20m; // 20%

    public bool SeAplica(decimal subtotal, string clienteEmail)
    {
        return !string.IsNullOrWhiteSpace(clienteEmail) &&
               clienteEmail.Contains(SufixoIdentificadorVip, StringComparison.OrdinalIgnoreCase);
    }

    public decimal CalcularDesconto(decimal subtotal)
    {
        return subtotal * PercentualDesconto;
    }
}
