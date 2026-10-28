namespace PiorEcommerceDoMundo.Domain.Strategies.Descontos;

/// <summary>
/// Estratégia padrão para quando nenhuma regra de desconto for aplicável (desconto zero).
/// </summary>
public class SemDescontoStrategy : IDescontoStrategy
{
    public bool SeAplica(decimal subtotal, string clienteEmail) => true;

    public decimal CalcularDesconto(decimal subtotal) => 0m;
}
