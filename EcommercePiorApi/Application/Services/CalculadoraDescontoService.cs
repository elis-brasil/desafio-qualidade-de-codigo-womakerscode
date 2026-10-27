using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Domain.Strategies.Descontos;

namespace PiorEcommerceDoMundo.Application.Services;

/// <summary>
/// Serviço responsável por coordenar a aplicação das estratégias de desconto (Strategy Pattern).
/// </summary>
public class CalculadoraDescontoService : ICalculadoraDesconto
{
    private readonly IEnumerable<IDescontoStrategy> _estrategias;
    private readonly ILogger<CalculadoraDescontoService> _logger;

    public CalculadoraDescontoService(
        IEnumerable<IDescontoStrategy> estrategias,
        ILogger<CalculadoraDescontoService> logger)
    {
        _estrategias = estrategias;
        _logger = logger;
    }

    public decimal CalcularDesconto(decimal subtotal, string clienteEmail)
    {
        foreach (var estrategia in _estrategias)
        {
            if (estrategia.SeAplica(subtotal, clienteEmail))
            {
                var desconto = estrategia.CalcularDesconto(subtotal);
                _logger.LogInformation("Estratégia {Estrategia} aplicada. Desconto: {Desconto:C2}",
                    estrategia.GetType().Name, desconto);
                return desconto;
            }
        }

        return 0m;
    }
}
