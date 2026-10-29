
using PiorEcommerceDoMundo;
using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Services;

public interface ILogisticaService
{
    Task ProcessarEnvioAsync(
        Pedido pedido,
        CancellationToken cancellationToken = default);
}
