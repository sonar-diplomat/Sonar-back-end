namespace Application.Abstractions.Interfaces.Services;

public interface IApiKeyGeneratorService
{
    Task<string> GenerateApiKey();
}