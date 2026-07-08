using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using PromptingDemo.Configuration;
using PromptingDemo.Models;

namespace PromptingDemo.Techniques;

public class ChainOfThought : IPromptingTechnique
{
    private readonly IChatClient _client;
    private readonly AssistantSettings _assistant;

    public ChainOfThought(IChatClient client, IOptions<AssistantSettings> assistant)
    {
        _client = client;
        _assistant = assistant.Value;
    }

    public string Name => "Chain-of-Thought";

    public async Task<string> ExecuteAsync(string userQuery, List<HistoryItem>? history = null, CancellationToken cancellationToken = default)
    {
        var contexto = BuildContext(history);

        var prompt = $$"""
            Eres un asistente de {{_assistant.Domain}}.

            {{contexto}}
            Para la siguiente consulta, razona paso a paso antes de dar tu respuesta final.
            Estructura tu respuesta así:

            Análisis: [evalúa el objetivo del usuario, su nivel y qué necesita considerar]
            Razonamiento: [explica qué ejercicios, series o enfoque se ajustan mejor y por qué]
            Recomendación final: [da la rutina o respuesta concreta, con series y repeticiones si aplica]

            Consulta: {{userQuery}}
            """;

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
        return $"Contexto de la conversación previa: {resumen}";
    }
}