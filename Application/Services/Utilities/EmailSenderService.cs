using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Abstractions.Interfaces.Services.Utilities;
using Entities.Enums;

namespace Application.Services.Utilities;

public class MailgunSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}

public class MailgunEmailService : IEmailSenderService
{
    private readonly HttpClient client;
    private readonly MailgunSettings settings;


    public MailgunEmailService(MailgunSettings options, HttpClient httpClient)
    {
        settings = options;
        client = httpClient;
    }

    public async Task SendEmailAsync(string to,
        string template,
        Dictionary<string, string>? variables = null)
    {
        if (!MailGunTemplates.IsValidTemplate(template)) throw new NotImplementedException();

        try
        {
            string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{settings.ApiKey}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(@"Basic", base64String);


            MultipartFormDataContent postData = new();
            postData.Add(new StringContent(settings.From), "from");
            postData.Add(new StringContent(to), "to");
            postData.Add(new StringContent(template), "template");

            postData.Add(new StringContent(JsonSerializer.Serialize(variables)), "t:variables");

            using HttpResponseMessage request =
                await client.PostAsync("https://api.mailgun.net/v3/" + settings.Domain + "/messages", postData);
            string response = await request.Content.ReadAsStringAsync();
        }
        catch (System.Exception e)
        {
            throw new NotImplementedException(e.Message);
        }
    }
}
