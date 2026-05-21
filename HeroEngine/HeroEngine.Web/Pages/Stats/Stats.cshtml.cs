using HeroEngine.Core.Data;
using HeroEngine.Web.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class StatsPageModel : PageModel
{
    private readonly HeroRepository _repo;
    private readonly CsvStatsWriter _csv;

    public int TotalHeroes { get; set; }
    public Dictionary<string, int> ClassDistribution { get; set; } = new();
    public List<HeroDto> TopHeroes { get; set; } = new();
    public Dictionary<string, int> AbilityTypeCounts { get; set; } = new();
    public List<string[]> CombatHistory { get; set; } = new();
    public List<HeroDto> SearchResults { get; set; } = new();
    public string SearchPattern { get; set; } = "";
    public string Filter { get; set; } = "";

    public StatsPageModel(HeroRepository repo, CsvStatsWriter csv)
    {
        _repo = repo;
        _csv = csv;
    }

    public void OnGet(string? filter = "", string? search = "")
    {
        Filter = filter ?? "";
        SearchPattern = search ?? "";

        var heroes = _repo.LoadAll();
        TotalHeroes = heroes.Count;

        var analytics = new HeroAnalytics(heroes);

        // Class distribution
        ClassDistribution = heroes
            .GroupBy(h => h.Type)
            .ToDictionary(g => g.Key, g => g.Count());

        // Top 3 by level
        // Cast to the expected element type because HeroAnalytics methods return non-generic IEnumerable
        TopHeroes = analytics.GetTopHeroesByLevel(3).ToList();

        // Ability type counts
        AbilityTypeCounts = heroes
            .SelectMany(h => h.Abilities)
            .GroupBy(a => a.Type)
            .ToDictionary(g => g.Key, g => g.Count());

        // Combat history from CSV with optional filter
        var all = _csv.ReadLast(100);
        CombatHistory = string.IsNullOrEmpty(Filter)
            ? all.TakeLast(10).ToList()
            : all.Where(r => r.Length > 3 &&
                             r[3].Contains(Filter, StringComparison.OrdinalIgnoreCase))
                 .TakeLast(10).ToList();

        // Regex search
        if (!string.IsNullOrEmpty(SearchPattern))
        {
            try
            {
                SearchResults = analytics.SearchHeroesByName(SearchPattern).ToList();
            }
            catch
            {
                SearchResults = new List<HeroDto>();
            }
        }
    }
}
