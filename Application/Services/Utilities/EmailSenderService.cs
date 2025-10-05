using System.Net.Http.Headers;
using System.Text;
using Application.Abstractions.Interfaces.Service.Utilities;
using Entities.Enums;
using Microsoft.Extensions.Options;

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


    public MailgunEmailService(IOptions<MailgunSettings> options, HttpClient httpClient)
    {
        settings = options.Value;
        client = httpClient;
    }

    public async Task SendEmailAsync(
        string to,
        string template,
        Dictionary<string, string>? variables = null)
    {
        if (!MailGunTemplates.IsValidTemplate(template)) throw new NotImplementedException();

        string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{settings.ApiKey}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(@"Basic", base64String);


        MultipartFormDataContent postData = new();
        postData.Add(new StringContent("string"), "from");
        postData.Add(new StringContent("string"), "to");
        postData.Add(new StringContent("string"), "subject");
        postData.Add(new StringContent("string"), "template");

        postData.Add(new StringContent("string"), "t:variables");

        using HttpResponseMessage request =
            await client.PostAsync("https://api.mailgun.net/v3/" + settings.Domain + "/messages", postData);
        string response = await request.Content.ReadAsStringAsync();

        Console.WriteLine(response);
    }
}