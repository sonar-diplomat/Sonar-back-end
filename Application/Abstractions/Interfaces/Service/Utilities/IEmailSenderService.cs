namespace Application.Abstractions.Interfaces.Service.Utilities;

public interface IEmailSenderService
{
    public Task SendEmailAsync(string to, string template, Dictionary<string, string>? variables = null);

    //Task SendVeryficationEmailAsync(string toEmail, string );
}