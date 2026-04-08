using AuditorPRO.Domain.Interfaces;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace AuditorPRO.Infrastructure.Services;

public class AzureOpenAIService : IAzureOpenAIService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName;
    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(IConfiguration config, ILogger<AzureOpenAIService> logger)
    {
        _logger = logger;
        var endpoint = config["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("AzureOpenAI:Endpoint not configured");
        var apiKey = config["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("AzureOpenAI:ApiKey not configured");
        _deploymentName = config["AzureOpenAI:DeploymentName"] ?? "gpt-4o";
        _client = new AzureOpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
    }

    public async Task<string> AnalizarControlAsync(string contexto, string descripcionControl, string resultado, CancellationToken ct = default)
    {
        var prompt = $"""
            Contexto organizacional: {contexto}

            Control auditado: {descripcionControl}
            Resultado de evaluación: {resultado}

            Analiza este resultado de auditoría. Identifica el riesgo, el impacto potencial
            y proporciona una recomendación específica y accionable.
            Incluye referencia a la norma aplicable (ISO 27001, COBIT, COSO).
            """;

        return await CallAsync(prompt, ct);
    }

    public async Task<string> GenerarRecomendacionAsync(string hallazgo, string dominio, CancellationToken ct = default)
    {
        var prompt = $"""
            Dominio de auditoría: {dominio}
            Hallazgo identificado: {hallazgo}

            Genera un plan de acción correctiva detallado con:
            1. Acción inmediata (primeras 48 horas)
            2. Acciones de corto plazo (30 días)
            3. Controles preventivos permanentes
            4. Métricas de seguimiento
            """;

        return await CallAsync(prompt, ct);
    }

    public async Task<string> ConsultarAsync(string pregunta, string contextoOrganizacional, CancellationToken ct = default)
        => await CallAsync(pregunta, ct, contextoOrganizacional);

    private async Task<string> CallAsync(string userMessage, CancellationToken ct, string? systemMessage = null)
    {
        try
        {
            var chatClient = _client.GetChatClient(_deploymentName);
            var messages = new List<ChatMessage>();

            if (systemMessage != null)
                messages.Add(new SystemChatMessage(systemMessage));

            messages.Add(new UserChatMessage(userMessage));

            var completion = await chatClient.CompleteChatAsync(messages, cancellationToken: ct);
            return completion.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Azure OpenAI");
            throw;
        }
    }
}
