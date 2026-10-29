using PiorEcommerceDoMundo;
using PiorEcommerceDoMundo.Domain.Entities;

namespace EcommercePiorApi.Services.Transportadora;

public interface ITransportadoraService
{
    Task<string> EnviarAsync(
        Pedido pedido,
        CancellationToken cancellationToken = default);
}
