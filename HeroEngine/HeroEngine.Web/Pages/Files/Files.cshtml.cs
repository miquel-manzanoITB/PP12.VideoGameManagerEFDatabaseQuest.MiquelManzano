using HeroEngine.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

public class FilesPageModel : PageModel
{
    private readonly HeroRepository _repo;
    private readonly CsvStatsWriter _csv;
    private readonly IWebHostEnvironment _env;

    private string ConfigPath => Path.Combine(_env.ContentRootPath, "Data", "game_config.xml");

    public string Message { get; set; } = "";
    public bool IsError { get; set; }
    public string HeroesJson { get; set; } = "";
    public string ConfigXml { get; set; } = "";
    public List<string[]> CombatRows { get; set; } = new();

    [BindProperty]
    public GameConfig Config { get; set; } = new();

    public FilesPageModel(HeroRepository repo, CsvStatsWriter csv, IWebHostEnvironment env)
    {
        _repo = repo;
        _csv = csv;
        _env = env;
    }

    public void OnGet()
    {
        LoadAll();
    }

    public IActionResult OnPostSaveConfig()
    {
        try
        {
            Config.Save(ConfigPath);
            Message = "Game config saved successfully.";
        }
        catch (Exception ex)
        {
            Message = $"Error saving config: {ex.Message}";
            IsError = true;
        }
        LoadAll();
        return Page();
    }

    public IActionResult OnPostDownloadCsv()
    {
        string path = Path.Combine(_env.ContentRootPath, "Data", "combat_stats.csv");
        if (!System.IO.File.Exists(path))
        {
            Message = "CSV file not found.";
            IsError = true;
            LoadAll();
            return Page();
        }
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        return File(bytes, "text/csv", "combat_stats.csv");
    }

    public IActionResult OnPostDownloadJson()
    {
        string path = Path.Combine(_env.ContentRootPath, "Data", "heroes.json");
        if (!System.IO.File.Exists(path))
        {
            Message = "heroes.json not found.";
            IsError = true;
            LoadAll();
            return Page();
        }
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        return File(bytes, "application/json", "heroes.json");
    }

    private void LoadAll()
    {
        // Load config
        Config = GameConfig.Load(ConfigPath);

        // Load heroes JSON
        try
        {
            var heroes = _repo.LoadAll();
            HeroesJson = JsonSerializer.Serialize(heroes,
                new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex) { HeroesJson = $"Error: {ex.Message}"; }

        // Load raw XML
        try
        {
            if (System.IO.File.Exists(ConfigPath))
                ConfigXml = System.IO.File.ReadAllText(ConfigPath);
        }
        catch (Exception ex) { ConfigXml = $"Error: {ex.Message}"; }

        // Load CSV rows
        CombatRows = _csv.ReadLast(10);
    }
}
