
namespace PiorEcommerceDoMundo.Services;

public class RastreamentoService : IRastreamentoService
{
    public string GerarCodigo()
    {
        return $"BR{Random.Shared.Next(100000, 1000000)}";
    }
}
