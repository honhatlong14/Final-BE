using Service.Utility;

namespace Service.IServices;

public interface ISendMailService
{
    Task SendMail(MailContent mailContent);
    
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}