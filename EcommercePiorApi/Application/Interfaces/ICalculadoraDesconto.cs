namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface ICalculadoraDesconto
{
    decimal CalcularDesconto(decimal subtotal, string clienteEmail);
}
