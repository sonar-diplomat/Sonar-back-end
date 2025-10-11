namespace Application.Abstractions.Interfaces.Services.Utilities;

public interface IEmailSenderService
{
    public Task SendEmailAsync(string to, string template, Dictionary<string, string>? variables = null);

    //Task SendVeryficationEmailAsync(string toEmail, string );
}
