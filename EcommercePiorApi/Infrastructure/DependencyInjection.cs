using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Infrastructure.Data;

namespace PiorEcommerceDoMundo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}