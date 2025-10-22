using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Response;
using Entities.Enums;

namespace Application.Services.Utilities;

public class MailgunSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}

public class MailgunEmailService(MailgunSettings options, HttpClient httpClient) : IEmailSenderService
{
    public async Task SendEmailAsync(string to,
        string template,
        Dictionary<string, string>? variables = null)
    {
        if (!MailGunTemplates.IsValidTemplate(template))
            throw ResponseFactory.Create<ExpectationFailedResponse>(["Invalid email template"]);

        string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{options.ApiKey}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(@"Basic", base64String);

        MultipartFormDataContent postData = new();
        postData.Add(new StringContent(options.From), "from");
        postData.Add(new StringContent(to), "to");
        postData.Add(new StringContent(template), "template");

        postData.Add(new StringContent(JsonSerializer.Serialize(variables)), "t:variables");

        using HttpResponseMessage request =
            await httpClient.PostAsync("https://api.mailgun.net/v3/" + options.Domain + "/messages", postData);
        string response = await request.Content.ReadAsStringAsync();
        if (!request.IsSuccessStatusCode)
            throw ResponseFactory.Create<ExpectationFailedResponse>([$"Failed to send email: {response}"]);
    }
}