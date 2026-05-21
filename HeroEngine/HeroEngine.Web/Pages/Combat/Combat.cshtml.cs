using HeroEngine.Core.Combat;
using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using HeroEngine.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public class CombatPageModel : PageModel
{
    private readonly HeroRepository _repo;
    private readonly CsvStatsWriter _csv;
    private readonly GameConfig _config;
    private readonly string _logPath;

    public List<HeroDto> AvailableHeroes { get; set; } = new();
    public string CombatLog { get; set; } = "";
    public string Message { get; set; } = "";
    public bool Victory { get; set; }
    public CombatResultDto? LastStats { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Please select at least one hero.")]
    public string SelectedHero1 { get; set; } = "";

    [BindProperty]
    public string SelectedHero2 { get; set; } = "";

    [BindProperty]
    public string EnemyType { get; set; } = "Minion";

    public CombatPageModel(HeroRepository repo, CsvStatsWriter csv,
                           GameConfig config, IWebHostEnvironment env)
    {
        _repo = repo;
        _csv = csv;
        _config = config;
        _logPath = Path.Combine(env.ContentRootPath, "Data", "battle.log");
    }

    public void OnGet()
    {
        if (System.IO.File.Exists(_logPath))
            CombatLog = System.IO.File.ReadAllText(_logPath);

        AvailableHeroes = _repo.LoadAll();
        LoadLog();
    }

    public IActionResult OnPost()
    {
        AvailableHeroes = _repo.LoadAll();

        if (!ModelState.IsValid)
        {
            LoadLog();
            return Page();
        }

        // Build hero list
        var heroDtos = new List<HeroDto>();
        var h1 = AvailableHeroes.FirstOrDefault(h =>
            h.Name.Equals(SelectedHero1, StringComparison.OrdinalIgnoreCase));
        if (h1 != null) heroDtos.Add(h1);

        if (!string.IsNullOrEmpty(SelectedHero2))
        {
            var h2 = AvailableHeroes.FirstOrDefault(h =>
                h.Name.Equals(SelectedHero2, StringComparison.OrdinalIgnoreCase)
                && h.Name != SelectedHero1);
            if (h2 != null) heroDtos.Add(h2);
        }

        if (!heroDtos.Any())
        {
            ModelState.AddModelError("", "Selected hero not found.");
            LoadLog();
            return Page();
        }

        // Convert DTOs to Hero objects
        var heroes = heroDtos.Select(BuildHero).ToList();
        var combatants = heroes.Select(h => (ICombatant)new HeroCombatant(h)).ToList();

        // Build enemies
        var enemies = new List<ICombatant>();
        int count = heroes.Count;
        for (int i = 1; i <= count; i++)
        {
            enemies.Add(EnemyType switch
            {
                "Elite" => new Elite($"Elite-{i}"),
                "Boss" => new Boss($"Boss-{i}"),
                _ => new Minion($"Minion-{i}")
            });
        }

        // Run combat with log
        var log = new List<string>();
        var helper = new CombatHelper();
        int round = 0;
        int totalDamage = 0;

        var activeHeroes = combatants.ToList();
        var activeEnemies = enemies.ToList();

        log.Add($"=== COMBAT LOG — {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
        log.Add($"Participants: {string.Join(", ", heroDtos.Select(h => h.Name))} vs {count}x {EnemyType}");
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

        // Write log (append)
        try
        {
            System.IO.File.AppendAllLines(_logPath, log);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Log write error: {ex.Message}");
        }

        // Find MVP (most damage among heroes)
        string mvp = heroDtos.First().Name;
        int mvpDmg = 0;
        foreach (var h in heroDtos)
        {
            int d = helper.GetTotalDamage(h.Name);
            if (d > mvpDmg) { mvpDmg = d; mvp = h.Name; }
        }

        // Save CSV stats
        var result = new CombatResultDto
        {
            Heroes = heroDtos.Select(h => h.Name).ToList(),
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
        catch (Exception ex)
        {
            CombatLog = $"Error reading log: {ex.Message}";
        }
    }

    private static Hero BuildHero(HeroDto dto) => dto.Type switch
    {
        "Mage" => new Mage(dto.Name, dto.Level),
        "Rogue" => new Rogue(dto.Name, dto.Level),
        _ => new Warrior(dto.Name, dto.Level)
    };
}
