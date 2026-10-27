namespace PiorEcommerceDoMundo.Domain.Strategies.Descontos;

public interface IDescontoStrategy
{
    bool SeAplica(decimal subtotal, string clienteEmail);
    decimal CalcularDesconto(decimal subtotal);
}
