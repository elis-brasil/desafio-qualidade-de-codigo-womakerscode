using PiorEcommerceDoMundo.Application.DTOs;

namespace PiorEcommerceDoMundo.Application.Interfaces;

public interface IPedidoService
{
    Task<PedidoResponse> CriarAsync(PedidoRequest request);
    Task<List<PedidoResponse>> ListarAsync();
    Task<PedidoResponse> ObterPorIdAsync(int id);
}
