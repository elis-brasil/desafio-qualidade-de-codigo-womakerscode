using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Infrastructure.Data;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Application.Services;

namespace PiorEcommerceDoMundo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPagamentoService, PagamentoService>();

        return services;
    }
}