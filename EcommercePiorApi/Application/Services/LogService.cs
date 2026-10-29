
namespace PiorEcommerceDoMundo.Services;

public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;

    public LogService(ILogger<LogService> logger)
    {
        _logger = logger;
    }

    public void Log(string mensagem)
    {
        _logger.LogInformation("{Mensagem}", mensagem);

        try
        {
            File.AppendAllText(
                "log.txt",
                $"{DateTimeOffset.Now:O} - {mensagem}"
                    + Environment.NewLine);
        }
        catch (IOException ex)
        {
            _logger.LogError(
                ex,
                "Não foi possível gravar a mensagem no arquivo de log.");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(
                ex,
                "Sem permissão para gravar no arquivo de log.");
        }
    }
}
