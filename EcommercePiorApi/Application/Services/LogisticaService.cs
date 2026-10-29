using EcommercePiorApi.Services.Transportadora;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo;
using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Domain.Enums;
using PiorEcommerceDoMundo.Infrastructure.Data;

namespace PiorEcommerceDoMundo.Services;

public class LogisticaService : ILogisticaService
{
    private readonly AppDbContext _db;
    private readonly ITransportadoraService _transportadora;
    private readonly ILogService _logService;

    public LogisticaService(
        AppDbContext db,
        ITransportadoraService transportadora,
        ILogService logService)
    {
        _db = db;
        _transportadora = transportadora;
        _logService = logService;
    }

    public async Task ProcessarEnvioAsync(
        Pedido pedido,
        CancellationToken cancellationToken = default)
    {
        var codigo = await _transportadora.EnviarAsync(
            pedido, cancellationToken);

        pedido.CodigoRastreio = codigo;
        pedido.Status = StatusPedido.Enviado;

        await _db.SaveChangesAsync(cancellationToken);

        _logService.Log(
            $"Pedido {pedido.Id} enviado. Rastreamento: {codigo}");
    }
}
