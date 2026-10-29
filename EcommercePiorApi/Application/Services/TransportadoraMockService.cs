using EcommercePiorApi.Services.Transportadora;
using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Infrastructure.Mocks;

namespace PiorEcommerceDoMundo.Infrastructure.Services
{
    public class TransportadoraMockService : ITransportadoraService
    {
        public async Task<string> EnviarAsync(Pedido pedido, CancellationToken cancellationToken = default)
        {
            return await LogisticaMock.EnviarAsync(pedido);
        }
    }
}