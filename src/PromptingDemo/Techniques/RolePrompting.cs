using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using PromptingDemo.Configuration;
using PromptingDemo.Models;

namespace PromptingDemo.Techniques;

public class RolePrompting : IPromptingTechnique
{
    private readonly IChatClient _client;
    private readonly AssistantSettings _assistant;

    public RolePrompting(IChatClient client, IOptions<AssistantSettings> assistant)
    {
        _client = client;
        _assistant = assistant.Value;
    }

    public string Name => "Role Prompting";

    public async Task<string> ExecuteAsync(string userQuery, List<HistoryItem>? history = null, CancellationToken cancellationToken = default)
    {
        var systemPrompt = $$"""
    Actúa como un entrenador personal certificado con 15 años de experiencia en {{_assistant.Domain}}.

    Tu estilo de comunicación:
    - Usas lenguaje claro y motivador, sin tecnicismos innecesarios.
    - Empatizas brevemente con el usuario antes de dar la recomendación.
    - Das instrucciones numeradas y fáciles de seguir, con series y repeticiones cuando aplique.
    - Si la información del usuario es insuficiente (nivel, objetivo, equipo disponible), pides aclaración antes de asumir.
    - Si el usuario menciona dolor, lesión o alguna condición médica, recuérdale amablemente que consulte a un médico o profesional de la salud antes de continuar con esa rutina.
    """;

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt)
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