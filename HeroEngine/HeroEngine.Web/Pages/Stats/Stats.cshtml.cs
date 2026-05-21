using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class StatsPageModel : PageModel
{
    private readonly HeroEngineContext _context;

    public int TotalHeroes { get; set; }
    public List<HeroEntity> TopHeroes { get; set; } = new();          // Tasca 6.2
    public List<HeroEntity> FilteredHeroes { get; set; } = new();     // Tasca 6.1
    public List<HeroEntity> SearchResults { get; set; } = new();      // Tasca 6.3

    // Tasca 7
    public List<dynamic> HeroesByClass { get; set; } = new();         // 7.1
    public List<dynamic> AbilitiesByRarity { get; set; } = new();     // 7.2

    public string SelectedClass { get; set; } = "";
    public string SearchTerm { get; set; } = "";
    public int? MinLevel { get; set; }
    public int TopN { get; set; } = 3;

    public StatsPageModel(HeroEngineContext context) => _context = context;

    public async Task OnGetAsync(
        string? selectedClass = "",
        string? search = "",
        int? minLevel = null,
        int topN = 3)
    {
        SelectedClass = selectedClass ?? "";
        SearchTerm = search ?? "";
        MinLevel = minLevel;
        TopN = topN > 0 ? topN : 3;

        TotalHeroes = await _context.Heroes.CountAsync();

        // Tasca 6.2 — Top N herois per nivell
        TopHeroes = await _context.Heroes
            .Include(h => h.HeroClass)
            .OrderByDescending(h => h.Level)
            .Take(TopN)
            .ToListAsync();

        // Tasca 6.1 — Filtratge per classe i ordenació per nivell
        if (!string.IsNullOrEmpty(SelectedClass))
        {
            FilteredHeroes = await _context.Heroes
                .Include(h => h.HeroClass)
                .Where(h => h.HeroClass!.Name == SelectedClass)
                .OrderByDescending(h => h.Level)
                .ToListAsync();
        }

        // Tasca 6.3 — Cerca per nom (sense distinció de majúscules)
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            SearchResults = await _context.Heroes
                .Include(h => h.HeroClass)
                .Where(h => h.Name.Contains(SearchTerm))
                .ToListAsync();
        }

        // Tasca 7.1 — Herois agrupats per classe amb recompte i nivell mitjà
        HeroesByClass = (await _context.Heroes
            .Include(h => h.HeroClass)
            .GroupBy(h => h.HeroClass!.Name)
            .Select(grp => new {
                ClassName = grp.Key,
                Count = grp.Count(),
                AvgLevel = grp.Average(h => (double)h.Level)
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync())
            .Cast<dynamic>()
            .ToList();

        // Tasca 7.2 — Habilitats per raresa
        AbilitiesByRarity = (await _context.Abilities
            .GroupBy(a => a.Rarity)
            .Select(grp => new { Rarity = grp.Key, Count = grp.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync())
            .Cast<dynamic>()
            .ToList();

        // Tasca 7.3 — Cerca combinada dinàmica (si s'usa el filtre combinat)
        if (!string.IsNullOrEmpty(SelectedClass) || MinLevel.HasValue)
        {
            var query = _context.Heroes.Include(h => h.HeroClass).AsQueryable();
            if (!string.IsNullOrEmpty(SelectedClass))
                query = query.Where(h => h.HeroClass!.Name == SelectedClass);
            if (MinLevel.HasValue)
                query = query.Where(h => h.Level >= MinLevel.Value);
            FilteredHeroes = await query.OrderBy(h => h.Name).ToListAsync();
        }
    }
}
