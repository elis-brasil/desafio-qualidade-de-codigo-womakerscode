namespace PiorEcommerceDoMundo.Infrastructure.Mocks;

// "servico" de pagamento que na verdade eh so uma classe estatica mockada
public static class PagamentoMock
{
    public static bool ProcessarPagamento(decimal valor, string formaPagamento)
    {
        Console.WriteLine("Processando pagamento de " + valor + " via " + formaPagamento);
        Thread.Sleep(500); // simula demora do gateway de pagamento, bloqueando a thread mesmo

        var random = new Random();
        var sorteio = random.Next(0, 100);

        // regra de negocio critica decidida no sorteio de um numero aleatorio
        if (formaPagamento == "cartao" && sorteio < 10)
        {
            return false;
        }
        return true;
    }
}