using Microsoft.AspNetCore.Mvc;

namespace PromptingDemo.Models
{
    public class HistoryItem
    {
        public string Technique { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }
}