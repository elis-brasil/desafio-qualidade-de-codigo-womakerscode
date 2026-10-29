using EcommercePiorApi.Services.Transportadora;
using PiorEcommerceDoMundo.Infrastructure.Services;
using PiorEcommerceDoMundo.Services;

namespace EcommercePiorApi.Api.Extensions
{
    public static class LogisticaExtensions
    {
        public static IServiceCollection AddLogistica(this IServiceCollection services)
        {
            services.AddScoped<ILogisticaService, LogisticaService>();
            services.AddScoped<IEmailValidatorService, EmailValidatorService>();
            services.AddScoped<IRastreamentoService, RastreamentoService>();
            services.AddScoped<ILogService, LogService>();

            services.AddScoped<ITransportadoraService, TransportadoraMockService>();

            return services;
        }
    }
}
