using System.Threading.Tasks;

namespace PLProj.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

}
