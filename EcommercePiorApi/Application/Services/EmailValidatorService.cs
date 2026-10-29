
using System.Net.Mail;

namespace PiorEcommerceDoMundo.Services;

public class EmailValidatorService : IEmailValidatorService
{
    public bool Validar(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var endereco = new MailAddress(email);

            return endereco.Address.Equals(
                email,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
