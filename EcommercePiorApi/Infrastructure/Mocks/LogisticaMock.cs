using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Shared;

namespace PiorEcommerceDoMundo.Infrastructure.Mocks;

// "integracao" com logistica que na verdade so espera um pouco e devolve um codigo aleatorio
public static class LogisticaMock
{
    public static async Task<string> EnviarAsync(Pedido pedido)
    {
        Console.WriteLine("Enviando pedido " + pedido.Id + " para a transportadora parceira...");
        await Task.Delay(300); // simula latencia da api externa de logistica
        return Utils.GerarCodigoRastreio();
    }
}