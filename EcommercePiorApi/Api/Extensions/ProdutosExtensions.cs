using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Application.Services;
using PiorEcommerceDoMundo.Infrastructure.Repositories;

namespace PiorEcommerceDoMundo.Api.Extensions;

public static class ProdutosExtensions
{
    public static IServiceCollection AddProdutos(this IServiceCollection services)
    {
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IEstoqueService, EstoqueService>();

        return services;
    }
}