using PiorEcommerceDoMundo.Application.Interfaces;

namespace PiorEcommerceDoMundo.Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PagamentoService> _logger;

    public PagamentoService(IConfiguration configuration, ILogger<PagamentoService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento)
    {
        if (valor <= 0) throw new ArgumentException("O valor do pagamento deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(formaPagamento)) throw new ArgumentException("A forma de pagamento é uma informação obrigatória.");

        _logger.LogInformation("Processando pagamento de {Valor} via {FormaPagamento}", valor, formaPagamento);

        var simularFalha =
            _configuration.GetValue<bool?>("Pagamento:SimularFalha")
            ?? throw new InvalidOperationException("A configuração Pagamento:SimularFalha não foi definida.");

        if (simularFalha)
        {
            _logger.LogWarning("Pagamento recusado para o valor {Valor}", valor);
            return Task.FromResult(false);
        }

        _logger.LogInformation("Pagamento aprovado para o valor {Valor}", valor);

        return Task.FromResult(true);
    }
}