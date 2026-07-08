using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using PromptingDemo.Configuration;
using PromptingDemo.Models;

namespace PromptingDemo.Techniques;

public class ZeroShot : IPromptingTechnique
{
    private readonly IChatClient _client;
    private readonly AssistantSettings _assistant;

    public ZeroShot(IChatClient client, IOptions<AssistantSettings> assistant)
    {
        _client = client;
        _assistant = assistant.Value;
    }

    public string Name => "Zero-shot";

    public async Task<string> ExecuteAsync(string userQuery, List<HistoryItem>? history = null, CancellationToken cancellationToken = default)
    {
        var contexto = BuildContext(history);
        var prompt = $"Eres un asistente experto en {_assistant.Domain}. {contexto}Responde la siguiente consulta de forma clara y directa: {userQuery}";
        var response = await _client.GetResponseAsync(prompt, cancellationToken: cancellationToken);
        return response.Text ?? string.Empty;
    }

    private static string BuildContext(List<HistoryItem>? history)
    {
        if (history is null || history.Count == 0)
        {
            return string.Empty;
        }

        var resumen = string.Join(" ", history.TakeLast(3).Select(h => $"El usuario preguntó: \"{h.Query}\" y se le respondió: \"{h.Response}\"."));
        return $"Contexto de la conversación previa: {resumen} ";
    }
}