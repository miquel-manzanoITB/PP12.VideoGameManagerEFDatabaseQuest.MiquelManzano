using HeroEngine.Core.Combat;
using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using HeroEngine.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class CombatPageModel : PageModel
{
    private readonly HeroEngineContext _context;   // ← BD en lloc de HeroRepository
    private readonly CsvStatsWriter _csv;
    private readonly GameConfig _config;
    private readonly string _logPath;

    public List<HeroEntity> AvailableHeroes { get; set; } = new();
    public string CombatLog { get; set; } = "";
    public string Message { get; set; } = "";
    public bool Victory { get; set; }
    public CombatResultDto? LastStats { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Selecciona almenys un heroi.")]
    public string SelectedHero1 { get; set; } = "";

    [BindProperty]
    public string SelectedHero2 { get; set; } = "";

    [BindProperty]
    public string EnemyType { get; set; } = "Minion";

    public CombatPageModel(HeroEngineContext context, CsvStatsWriter csv,
                           GameConfig config, IWebHostEnvironment env)
    {
        _context = context;
        _csv = csv;
        _config = config;
        _logPath = Path.Combine(env.ContentRootPath, "Data", "battle.log");
    }

    public async Task OnGetAsync()
    {
        AvailableHeroes = await _context.Heroes
            .Include(h => h.HeroClass)
            .Include(h => h.Abilities)
            .ToListAsync();
        LoadLog();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AvailableHeroes = await _context.Heroes
            .Include(h => h.HeroClass)
            .Include(h => h.Abilities)
            .ToListAsync();

        if (!ModelState.IsValid)
        {
            LoadLog();
            return Page();
        }

        // Build hero list from DB entities
        var selectedEntities = new List<HeroEntity>();

        var h1 = AvailableHeroes.FirstOrDefault(h =>
            h.Name.Equals(SelectedHero1, StringComparison.OrdinalIgnoreCase));
        if (h1 != null) selectedEntities.Add(h1);

        if (!string.IsNullOrEmpty(SelectedHero2))
        {
            var h2 = AvailableHeroes.FirstOrDefault(h =>
                h.Name.Equals(SelectedHero2, StringComparison.OrdinalIgnoreCase)
                && h.Name != SelectedHero1);
            if (h2 != null) selectedEntities.Add(h2);
        }

        if (!selectedEntities.Any())
        {
            ModelState.AddModelError("", "Heroi seleccionat no trobat.");
            LoadLog();
            return Page();
        }

        // Convert DB entities to combat Hero objects (Warrior, Mage, Rogue)
        // This is where the OOP hierarchy kicks in
        var heroes = selectedEntities.Select(BuildCombatHero).ToList();
        var combatants = heroes.Select(h => (ICombatant)new HeroCombatant(h)).ToList();

        // Build enemies
        int count = heroes.Count;
        var enemies = new List<ICombatant>();
        for (int i = 1; i <= count; i++)
        {
            enemies.Add(EnemyType switch
            {
                "Elite" => new Elite($"Elite-{i}"),
                "Boss" => new Boss($"Boss-{i}"),
                _ => new Minion($"Minion-{i}")
            });
        }

        // ─── Combat loop (same as before) ────────────────────────────────────
        var log = new List<string>();
        var helper = new CombatHelper();
        int round = 0, totalDamage = 0;

        var activeHeroes = combatants.ToList();
        var activeEnemies = enemies.ToList();

        log.Add($"=== COMBAT LOG — {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
        log.Add($"Participants: {string.Join(", ", selectedEntities.Select(h => h.Name))} vs {count}x {EnemyType}");
        log.Add(new string('-', 50));

        int maxRounds = _config.MaxCombatRounds;
        while (round < maxRounds
               && activeHeroes.Any(h => !h.IsDefeated)
               && activeEnemies.Any(e => !e.IsDefeated))
        {
            round++;
            log.Add($"--- Round {round} ---");

            var turnOrder = activeHeroes.Concat(activeEnemies)
                                        .Where(c => !c.IsDefeated)
                                        .OrderByDescending(c => c.Initiative);

            foreach (var attacker in turnOrder)
            {
                if (attacker.IsDefeated) continue;

                var targets = activeHeroes.Contains(attacker)
                    ? activeEnemies.Where(e => !e.IsDefeated).ToList()
                    : activeHeroes.Where(h => !h.IsDefeated).ToList();

                if (!targets.Any()) break;

                var target = targets.OrderBy(t => t is HeroCombatant hc
                    ? hc.UnderlyingHero.CurrentHp : ((Enemy)t).CurrentHp).First();

                int dmg = attacker.Attack();
                int net = target.ReceiveDamage(dmg);
                totalDamage += net;

                string side = activeHeroes.Contains(attacker) ? "[HERO] " : "[ENEMY]";
                log.Add($"  {side} {attacker.Name,-12} -> {target.Name,-14} -> {net} dmg"
                        + (target.IsDefeated ? " | DEFEATED!" : ""));
                helper.RecordAction(attacker.Name, net, round, target.IsDefeated);
            }
        }

        bool victory = activeEnemies.All(e => e.IsDefeated);
        log.Add(new string('-', 50));
        log.Add(victory ? "Result: VICTORY" : "Result: DEFEAT");
        log.Add(new string('=', 50));

        try { System.IO.File.AppendAllLines(_logPath, log); }
        catch (Exception ex) { Console.Error.WriteLine($"Log write error: {ex.Message}"); }

        // Find MVP
        string mvp = selectedEntities.First().Name;
        int mvpDmg = 0;
        foreach (var entity in selectedEntities)
        {
            int d = helper.GetTotalDamage(entity.Name);
            if (d > mvpDmg) { mvpDmg = d; mvp = entity.Name; }
        }

        // Save CSV stats
        var result = new CombatResultDto
        {
            Heroes = selectedEntities.Select(h => h.Name).ToList(),
            Enemies = Enumerable.Range(1, count).Select(i => $"{EnemyType}-{i}").ToList(),
            Victory = victory,
            Rounds = round,
            TotalDamage = totalDamage,
            MostEffective = mvp
        };
        _csv.AppendCombatStats(result);

        LastStats = result;
        Victory = victory;
        Message = victory
            ? $"🏆 Victory! Heroes triumphed in {round} rounds."
            : $"💀 Defeat! All heroes fell after {round} rounds.";

        LoadLog();
        return Page();
    }

    private void LoadLog()
    {
        try
        {
            if (System.IO.File.Exists(_logPath))
                CombatLog = System.IO.File.ReadAllText(_logPath);
        }
        catch (Exception ex) { CombatLog = $"Error reading log: {ex.Message}"; }
    }

    /// <summary>
    /// Converts a DB HeroEntity into the correct combat Hero subclass.
    /// This is where polymorphism kicks in: Warrior/Mage/Rogue.
    /// </summary>
    private static Hero BuildCombatHero(HeroEntity entity) =>
        entity.HeroClass?.Name switch
        {
            "Mage" => new Mage(entity.Name, entity.Level),
            "Rogue" => new Rogue(entity.Name, entity.Level),
            _ => new Warrior(entity.Name, entity.Level)
        };
}
