using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using PromptingDemo.Configuration;
using PromptingDemo.Models;

namespace PromptingDemo.Techniques;

public class FewShot : IPromptingTechnique
{
    private readonly IChatClient _client;
    private readonly AssistantSettings _assistant;

    public FewShot(IChatClient client, IOptions<AssistantSettings> assistant)
    {
        _client = client;
        _assistant = assistant.Value;
    }

    public string Name => "Few-shot";

    public async Task<string> ExecuteAsync(string userQuery, List<HistoryItem>? history = null, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, $"Eres un asistente de {_assistant.Domain}. Responde de forma breve, en máximo 3 pasos enumerados."),
            new(ChatRole.User, "Quiero una rutina básica de pecho."),
            new(ChatRole.Assistant, "1. Press de banca: 4 series de 8-10 repeticiones. 2. Aperturas con mancuernas: 3 series de 12 repeticiones. 3. Fondos en paralelas: 3 series de 10 repeticiones."),
            new(ChatRole.User, "¿Cómo hago sentadillas correctamente?"),
            new(ChatRole.Assistant, "1. Coloca los pies al ancho de los hombros. 2. Baja manteniendo la espalda recta y las rodillas alineadas con los pies. 3. Sube empujando con los talones sin bloquear las rodillas."),
            new(ChatRole.User, "Necesito una rutina para ganar resistencia cardiovascular."),
            new(ChatRole.Assistant, "1. Trote continuo: 20-30 minutos a ritmo moderado. 2. Saltos de cuerda: 3 series de 2 minutos. 3. Burpees: 3 series de 15 repeticiones.")
        };

        if (history is not null)
        {
            foreach (var item in history.TakeLast(3))
            {
                messages.Add(new ChatMessage(ChatRole.User, item.Query));
                messages.Add(new ChatMessage(ChatRole.Assistant, item.Response));
            }
        }

        messages.Add(new ChatMessage(ChatRole.User, userQuery));

        var response = await _client.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return response.Text ?? string.Empty;
    }
}