using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PromptingDemo.Configuration;
using PromptingDemo.Models;
using PromptingDemo.Techniques;

namespace PromptingDemo.Controllers;

public class HomeController : Controller
{
    private const string HistorySessionKey = "ChatHistory";
    private const string ProfileSessionKey = "UserProfile";

    private readonly IEnumerable<IPromptingTechnique> _techniques;
    private readonly OllamaSettings _ollamaSettings;

    public HomeController(IEnumerable<IPromptingTechnique> techniques, IOptions<OllamaSettings> ollamaSettings)
    {
        _techniques = techniques;
        _ollamaSettings = ollamaSettings.Value;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = BuildModel();
        model.History = GetHistory();
        model.Profile = GetProfile();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(string selectedTechnique, string query, string nivel, string objetivo)
    {
        var model = BuildModel();
        model.SelectedTechnique = selectedTechnique;
        model.Query = query;
        model.History = GetHistory();

        var profile = new UserProfile { Nivel = nivel ?? string.Empty, Objetivo = objetivo ?? string.Empty };
        SaveProfile(profile);
        model.Profile = profile;

        if (string.IsNullOrWhiteSpace(query))
        {
            model.Error = "Debe escribir una consulta.";
            return View(model);
        }

        var technique = _techniques.FirstOrDefault(t => t.Name == selectedTechnique);
        if (technique is null)
        {
            model.Error = "Técnica no encontrada.";
            return View(model);
        }

        try
        {
            var queryConPerfil = BuildQueryWithProfile(query, profile);
            model.Response = await technique.ExecuteAsync(queryConPerfil, model.History);

            var history = model.History;
            history.Add(new HistoryItem
            {
                Technique = selectedTechnique,
                Query = query,
                Response = model.Response
            });
            SaveHistory(history);
            model.History = history;
        }
        catch (Exception ex)
        {
            model.Error = ex.Message;
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult ClearHistory()
    {
        HttpContext.Session.Remove(HistorySessionKey);
        return RedirectToAction("Index");
    }

    private static string BuildQueryWithProfile(string query, UserProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.Nivel) && string.IsNullOrWhiteSpace(profile.Objetivo))
        {
            return query;
        }

        var contexto = $"(Nivel del usuario: {profile.Nivel}. Objetivo del usuario: {profile.Objetivo}.) {query}";
        return contexto;
    }

    private PromptViewModel BuildModel()
    {
        return new PromptViewModel
        {
            Model = _ollamaSettings.Model,
            BaseUrl = _ollamaSettings.BaseUrl,
            AvailableTechniques = _techniques.Select(t => t.Name).ToList(),
            SelectedTechnique = _techniques.First().Name
        };
    }

    private List<HistoryItem> GetHistory()
    {
        var json = HttpContext.Session.GetString(HistorySessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new List<HistoryItem>();
        }

        return JsonSerializer.Deserialize<List<HistoryItem>>(json) ?? new List<HistoryItem>();
    }

    private void SaveHistory(List<HistoryItem> history)
    {
        var json = JsonSerializer.Serialize(history);
        HttpContext.Session.SetString(HistorySessionKey, json);
    }

    private UserProfile GetProfile()
    {
        var json = HttpContext.Session.GetString(ProfileSessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new UserProfile();
        }

        return JsonSerializer.Deserialize<UserProfile>(json) ?? new UserProfile();
    }

    private void SaveProfile(UserProfile profile)
    {
        var json = JsonSerializer.Serialize(profile);
        HttpContext.Session.SetString(ProfileSessionKey, json);
    }
}