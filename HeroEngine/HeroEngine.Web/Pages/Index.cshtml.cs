using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly HeroEngineContext _context;
    private readonly CsvStatsWriter _csv;
    private readonly SyncService _sync;

    public int HeroCount { get; set; }
    public int WarriorCount { get; set; }
    public int MageCount { get; set; }
    public int RogueCount { get; set; }
    public int CombatCount { get; set; }
    public List<HeroEntity> RecentHeroes { get; set; } = new();
    public string SyncMessage { get; set; } = "";
    public bool SyncError { get; set; }

    public IndexModel(HeroEngineContext context, CsvStatsWriter csv, SyncService sync)
    {
        _context = context;
        _csv = csv;
        _sync = sync;
    }

    public async Task OnGetAsync()
    {
        await EnsureSeedAsync();
        await LoadStatsAsync();
    }

    // Botó: DB → Files
    public async Task<IActionResult> OnPostExportToFilesAsync()
    {
        try
        {
            SyncMessage = await _sync.ExportDbToFilesAsync();
        }
        catch (Exception ex)
        {
            SyncMessage = $"Error exportant: {ex.Message}";
            SyncError = true;
        }
        await LoadStatsAsync();
        return Page();
    }

    // Botó: Files → DB
    public async Task<IActionResult> OnPostImportFromFilesAsync()
    {
        try
        {
            SyncMessage = await _sync.ImportFilesToDbAsync();
        }
        catch (Exception ex)
        {
            SyncMessage = $"Error important: {ex.Message}";
            SyncError = true;
        }
        await LoadStatsAsync();
        return Page();
    }

    private async Task LoadStatsAsync()
    {
        var heroes = await _context.Heroes
            .Include(h => h.HeroClass)
            .ToListAsync();

        HeroCount = heroes.Count;
        WarriorCount = heroes.Count(h => h.HeroClass?.Name == "Warrior");
        MageCount = heroes.Count(h => h.HeroClass?.Name == "Mage");
        RogueCount = heroes.Count(h => h.HeroClass?.Name == "Rogue");
        RecentHeroes = heroes.TakeLast(5).ToList();
        CombatCount = _csv.ReadLast(100).Count;
    }

    private async Task EnsureSeedAsync()
    {
        if (await _context.HeroClasses.AnyAsync()) return;

        var warrior = new HeroClass { Name = "Warrior", Description = "Tanc melee d'alta defensa" };
        var mage = new HeroClass { Name = "Mage", Description = "Mag d'atac màgic" };
        var rogue = new HeroClass { Name = "Rogue", Description = "Assassí ràpid i sigilós" };
        _context.HeroClasses.AddRange(warrior, mage, rogue);
        await _context.SaveChangesAsync();

        _context.Heroes.AddRange(
            new HeroEntity { Name = "Aldric", Level = 5, MaxHp = 200, Armor = 60, HeroClassId = warrior.Id },
            new HeroEntity { Name = "Lyria", Level = 4, MaxHp = 120, Armor = 20, HeroClassId = mage.Id }
        );
        await _context.SaveChangesAsync();
    }
}
