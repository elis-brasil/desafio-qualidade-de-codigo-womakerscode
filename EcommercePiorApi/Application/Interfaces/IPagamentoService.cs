namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface IPagamentoService
{
    Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento);
}