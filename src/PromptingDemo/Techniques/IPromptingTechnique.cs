using PromptingDemo.Models;

namespace PromptingDemo.Techniques;

public interface IPromptingTechnique
{
    string Name { get; }
    Task<string> ExecuteAsync(string userQuery, List<HistoryItem>? history = null, CancellationToken cancellationToken = default);
}