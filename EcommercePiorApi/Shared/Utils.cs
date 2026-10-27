namespace PiorEcommerceDoMundo.Shared;

// classe utilitaria "faz-tudo": validacao, log e geracao de codigo no mesmo lugar
public static class Utils
{
    public static bool ValidarEmail(string email)
    {
        if (email == null) return false;
        return email.Contains("@");
    }

    public static void Log(string mensagem)
    {
        Console.WriteLine(mensagem);
        try
        {
            File.AppendAllText("log.txt", DateTime.Now + " - " + mensagem + Environment.NewLine);
        }
        catch
        {
            // ignora qualquer erro de log, log nunca pode quebrar a aplicacao (ou sera que pode?)
        }
    }

    public static string GerarCodigoRastreio()
    {
        var random = new Random();
        return "BR" + random.Next(100000, 999999);
    }
}