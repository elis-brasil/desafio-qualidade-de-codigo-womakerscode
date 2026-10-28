using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Application.Services;
using PiorEcommerceDoMundo.Domain.Strategies.Descontos;
using PiorEcommerceDoMundo.Infrastructure.Repositories;

namespace PiorEcommerceDoMundo.Api.Extensions;

public static class PedidosExtensions
{
    public static IServiceCollection AddPedidos(this IServiceCollection services)
    {
        // Repositório
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        // Estratégias de Desconto (Strategy Pattern - Tarefa #6)
        services.AddScoped<IDescontoStrategy, DescontoValorAltoStrategy>();
        services.AddScoped<IDescontoStrategy, DescontoClienteVipStrategy>();

        // Serviços de Domínio e Aplicação (Separação de cálculo e validação)
        services.AddScoped<ICalculadoraDesconto, CalculadoraDescontoService>();
        services.AddScoped<IPedidoValidator, PedidoValidator>();
        services.AddScoped<IPedidoService, PedidoService>();

        return services;
    }
}
