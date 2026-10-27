using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Application.Services;

namespace PiorEcommerceDoMundo.Api.Extensions;

public static class PagamentosExtensions
{
    public static IServiceCollection AddPagamentos(this IServiceCollection services)
    {
        services.AddScoped<IPagamentoService, PagamentoService>();

        return services;
    }
}