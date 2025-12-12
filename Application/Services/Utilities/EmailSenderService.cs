using Application.Abstractions.Interfaces.Services.Utilities;
using Application.Response;
using Entities.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using SysFile = System.IO.File;

namespace Application.Services.Utilities;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string? FromName { get; set; }
    public bool EnableSsl { get; set; } = true;
    public bool UseDefaultCredentials { get; set; } = false;
}

public class SmtpEmailService : IEmailSenderService
{
    private readonly SmtpSettings _settings;
    private readonly Dictionary<string, string> _templates;
    private readonly string _templatesPath;
    private readonly string _baseUrl;

    public SmtpEmailService(SmtpSettings settings, IConfiguration configuration)
    {
        _settings = settings;
        _templates = new Dictionary<string, string>();
        _baseUrl = configuration["FrontEnd-Url"] ?? throw new InvalidOperationException("FrontEnd-Url not found in configuration");

        // Определяем путь к шаблонам, пробуя несколько вариантов
        string[] possiblePaths =
        {
            // Для разработки (из bin/Debug)
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Application", "EmailTemplates"),
            // Для опубликованного приложения
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates"),
            // Относительно текущей рабочей директории
            Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates"),
            // Для Docker/опубликованного приложения в корне
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Application", "EmailTemplates")
        };

        _templatesPath = string.Empty;
        foreach (string path in possiblePaths)
        {
            string normalizedPath = Path.GetFullPath(path);
            if (Directory.Exists(normalizedPath))
            {
                _templatesPath = normalizedPath;
                break;
            }
        }

        // Если ни один путь не найден, используем первый как fallback
        if (string.IsNullOrEmpty(_templatesPath))
        {
            _templatesPath = possiblePaths[1]; // EmailTemplates в BaseDirectory
        }

        LoadTemplates();
    }

    private void LoadTemplates()
    {
        string[] templateFiles = { "2fa.html", "confirm-email.html", "recovery-password.html" };

        if (!Directory.Exists(_templatesPath))
        {
            throw new InvalidOperationException($"Email templates directory not found: {_templatesPath}");
        }

        foreach (string templateFile in templateFiles)
        {
            string templatePath = Path.Combine(_templatesPath, templateFile);
            if (SysFile.Exists(templatePath))
            {
                string templateName = Path.GetFileNameWithoutExtension(templateFile);
                _templates[templateName] = SysFile.ReadAllText(templatePath);
            }
            else
            {
                throw new FileNotFoundException($"Email template not found: {templatePath}");
            }
        }
    }

    private string ReplaceVariables(string template, Dictionary<string, string>? variables)
    {
        if (variables == null)
            return template;

        string result = template;
        foreach (KeyValuePair<string, string> variable in variables)
        {
            result = result.Replace($"{{{{{variable.Key}}}}}", variable.Value);
        }

        return result;
    }

    private string GetPlainTextFromHtml(string html)
    {
        // Простое преобразование HTML в plain text
        string text = html;
        text = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]+>", "");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }

    /// <summary>
    /// Формирует полную ссылку из базового URL, роута и параметров запроса.
    /// </summary>
    /// <param name="route">Роут (например, "confirm-email" или "/confirm-email")</param>
    /// <param name="queryParams">Параметры запроса (ключ-значение)</param>
    /// <returns>Полная ссылка</returns>
    public string BuildLink(string route, Dictionary<string, string>? queryParams = null)
    {
        // Убираем ведущий слэш, если есть
        route = route.TrimStart('/');

        // Формируем базовую ссылку
        string baseUrl = _baseUrl.TrimEnd('/');
        string fullLink = $"{baseUrl}/{route}";

        // Добавляем параметры запроса
        if (queryParams != null && queryParams.Count > 0)
        {
            var queryString = string.Join("&", queryParams
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            if (!string.IsNullOrEmpty(queryString))
            {
                fullLink += $"?{queryString}";
            }
        }

        return fullLink;
    }

    public async Task SendEmailAsync(string to, string template, Dictionary<string, string>? variables = null)
    {
        if (string.IsNullOrWhiteSpace(_settings.From))
            throw ResponseFactory.Create<ExpectationFailedResponse>(["Email sender address (Smtp:From) is empty."]);

        if (string.IsNullOrWhiteSpace(to))
            throw ResponseFactory.Create<ExpectationFailedResponse>(["Recipient email address is empty."]);

        if (!MailTemplates.IsValidTemplate(template))
            throw ResponseFactory.Create<ExpectationFailedResponse>(["Invalid email template"]);

        // Маппинг имен шаблонов на имена файлов
        string templateFileName = template switch
        {
            MailTemplates.twoFA => "2fa",
            MailTemplates.confirmEmail => "confirm-email",
            MailTemplates.passwordRecovery => "recovery-password",
            _ => template
        };

        if (!_templates.TryGetValue(templateFileName, out string? htmlTemplate))
            throw ResponseFactory.Create<ExpectationFailedResponse>([$"Template '{template}' not found"]);

        // Если в variables есть "route" и "linkParams", формируем полную ссылку
        if (variables != null && variables.ContainsKey("route"))
        {
            string route = variables["route"];
            variables.Remove("route");

            // Извлекаем параметры для ссылки (все переменные, начинающиеся с "linkParam_")
            Dictionary<string, string> linkParams = new();
            var linkParamKeys = variables.Keys.Where(k => k.StartsWith("linkParam_")).ToList();
            foreach (string key in linkParamKeys)
            {
                string paramName = key.Substring("linkParam_".Length);
                linkParams[paramName] = variables[key];
                variables.Remove(key);
            }

            // Если есть явные linkParams, используем их
            if (variables.ContainsKey("linkParams"))
            {
                // linkParams может быть JSON строкой или просто игнорироваться
                variables.Remove("linkParams");
            }

            // Формируем полную ссылку и добавляем в variables как "link"
            string fullLink = BuildLink(route, linkParams.Count > 0 ? linkParams : null);
            variables["link"] = fullLink;
        }

        string htmlBody = ReplaceVariables(htmlTemplate, variables);
        string plainTextBody = GetPlainTextFromHtml(htmlBody);

        using MailMessage message = new();
        message.From = new MailAddress(_settings.From, _settings.FromName ?? string.Empty);
        message.To.Add(new MailAddress(to));
        message.Subject = templateFileName switch
        {
            "2fa" => "Two-Factor Authentication Code",
            "confirm-email" => "Confirm Your Email Address",
            "recovery-password" => "Password Recovery",
            _ => "Email from Sonar"
        };

        // Используем AlternateView для правильной отправки multipart/alternative
        // Это гарантирует, что почтовые клиенты выберут HTML версию, если поддерживают
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, System.Text.Encoding.UTF8, "text/html");
        AlternateView plainTextView = AlternateView.CreateAlternateViewFromString(plainTextBody, System.Text.Encoding.UTF8, "text/plain");

        // Добавляем сначала plain text (fallback), затем HTML (предпочтительный)
        message.AlternateViews.Add(plainTextView);
        message.AlternateViews.Add(htmlView);

        // Также устанавливаем HTML как основное тело для совместимости
        message.Body = htmlBody;
        message.IsBodyHtml = true;
        message.BodyEncoding = System.Text.Encoding.UTF8;

        using SmtpClient client = new();
        client.Host = _settings.Host;
        client.Port = _settings.Port;
        client.EnableSsl = _settings.EnableSsl;
        client.UseDefaultCredentials = _settings.UseDefaultCredentials;

        if (!_settings.UseDefaultCredentials && !string.IsNullOrEmpty(_settings.Username))
        {
            client.Credentials = new System.Net.NetworkCredential(_settings.Username, _settings.Password);
        }

        try
        {
            await client.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            throw ResponseFactory.Create<ExpectationFailedResponse>([$"Failed to send email via SMTP: {ex.Message}"]);
        }
        catch (Exception ex)
        {
            throw ResponseFactory.Create<ExpectationFailedResponse>([$"Unexpected error sending email: {ex.Message}"]);
        }
    }
}