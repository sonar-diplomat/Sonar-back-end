namespace Application.Abstractions.Interfaces.Service.Utilities;

public interface IEmailSenderService
{
    Task SendEmailAsync(string toEmail, string subject, string message);
    //Task SendVeryficationEmailAsync(string toEmail, string );
}