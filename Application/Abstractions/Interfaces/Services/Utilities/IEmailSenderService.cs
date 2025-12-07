namespace Application.Abstractions.Interfaces.Services.Utilities;

public interface IEmailSenderService
{
    public Task SendEmailAsync(string to, string template, Dictionary<string, string>? variables = null);
    
    /// <summary>
    /// Формирует полную ссылку из базового URL, роута и параметров запроса.
    /// </summary>
    /// <param name="route">Роут (например, "confirm-email" или "/confirm-email")</param>
    /// <param name="queryParams">Параметры запроса (ключ-значение)</param>
    /// <returns>Полная ссылка</returns>
    public string BuildLink(string route, Dictionary<string, string>? queryParams = null);

    //Task SendVeryficationEmailAsync(string toEmail, string );
}
