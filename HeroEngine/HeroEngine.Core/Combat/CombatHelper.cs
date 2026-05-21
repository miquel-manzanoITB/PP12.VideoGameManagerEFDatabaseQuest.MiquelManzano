namespace HeroEngine.Core.Combat;

/// <summary>
/// Auxiliary class with single responsibility: tracking and reporting
/// combat statistics. SRP — the CombatEngine only calls RecordAction();
/// all stats logic is isolated here.
/// </summary>
public sealed class CombatHelper
{
    // Per-combatant total damage dealt
    private readonly Dictionary<string, int>  _totalDamage  = new();
    // First round in which a kill was scored per combatant
    private readonly Dictionary<string, int>  _killRounds   = new();
    // Names of enemies defeated, with the round number
    private readonly List<(string enemy, int round)> _defeats = new();

    /// <summary>
    /// Records a single combat action.
    /// </summary>
    /// <param name="attackerName">Name of the attacker.</param>
    /// <param name="damage">Net damage dealt.</param>
    /// <param name="round">Current round number.</param>
    /// <param name="targetDefeated">Whether the target was killed this action.</param>
    public void RecordAction(string attackerName, int damage, int round, bool targetDefeated)
    {
        if (!_totalDamage.ContainsKey(attackerName))
            _totalDamage[attackerName] = 0;

        _totalDamage[attackerName] += damage;

        if (targetDefeated)
            _defeats.Add((attackerName, round));
    }

    /// <summary>
    /// Prints a full statistics report after the battle ends.
    /// </summary>
    public void PrintStats()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║          COMBAT STATISTICS               ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        if (_totalDamage.Count == 0)
        {
            Console.WriteLine("  (no data recorded)");
            return;
        }

        // Total damage per combatant
        Console.WriteLine("\n  ── Damage Dealt ────────────────────────");
        foreach (var (name, dmg) in _totalDamage.OrderByDescending(kv => kv.Value))
            Console.WriteLine($"    {name,-20} {dmg,5} damage");

        // Most effective hero
        var (mvpName, mvpDmg) = _totalDamage.MaxBy(kv => kv.Value);
        Console.WriteLine($"\n  ⭐  Most effective: {mvpName} ({mvpDmg} total damage)");

        // Fastest kill (fewest rounds)
        if (_defeats.Count > 0)
        {
            var fastest = _defeats.MinBy(d => d.round);
            Console.WriteLine($"  ⚡  Fastest defeat by: {fastest.enemy} in round {fastest.round}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Returns the total damage dealt by a specific combatant.
    /// </summary>
    public int GetTotalDamage(string combatantName)
        => _totalDamage.TryGetValue(combatantName, out int v) ? v : 0;
}
