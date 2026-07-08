namespace PromptingDemo.Configuration;

public class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3.2:1b";
}

public class AssistantSettings
{
    public string Domain { get; set; } = string.Empty;
}
