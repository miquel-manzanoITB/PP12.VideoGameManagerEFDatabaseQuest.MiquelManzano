using HeroEngine.Core.Data;
using HeroEngine.Web.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly HeroRepository _repo;
    private readonly CsvStatsWriter _csv;

    public int HeroCount { get; set; }
    public int WarriorCount { get; set; }
    public int MageCount { get; set; }
    public int RogueCount { get; set; }
    public int CombatCount { get; set; }
    public List<HeroDto> RecentHeroes { get; set; } = new();

    public IndexModel(HeroRepository repo, CsvStatsWriter csv)
    {
        _repo = repo;
        _csv = csv;
    }

    public void OnGet()
    {
        var all = _repo.LoadAll();
        HeroCount = all.Count;
        WarriorCount = all.Count(h => h.Type == "Warrior");
        MageCount = all.Count(h => h.Type == "Mage");
        RogueCount = all.Count(h => h.Type == "Rogue");
        RecentHeroes = all.TakeLast(5).ToList();
        CombatCount = _csv.ReadLast(100).Count;
    }
}
