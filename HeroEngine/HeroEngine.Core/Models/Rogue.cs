namespace HeroEngine.Core.Models;

/// <summary>
/// A Rogue hero. Deals bonus stealth damage and can throw hidden daggers
/// for multi-target hits.
/// </summary>
public sealed class Rogue : Hero
{
    // ─── Rogue-specific stats ────────────────────────────────────────────────

    /// <summary>Multiplier applied to stealth attacks (1.0 = no bonus).</summary>
    public double StealthMultiplier { get; private set; }

    /// <summary>Number of hidden daggers available for multi-hit attacks.</summary>
    public int HiddenDaggers { get; private set; }

    private readonly int _baseAttack;
    private bool _stealthed = true;   // first attack is always a stealth strike
    private static readonly Random _rng = new();

    // ─── Constructor ─────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a Rogue, scaling stealth bonuses and daggers to <paramref name="level"/>.
    /// </summary>
    /// <param name="name">Rogue's name.</param>
    /// <param name="level">Starting level (≥ 1).</param>
    /// <param name="stealthMultiplier">Stealth damage multiplier (≥ 1.0).</param>
    public Rogue(string name, int level, double stealthMultiplier = 1.5)
        : base(name, level, baseHp: 80)
    {
        if (stealthMultiplier < 1.0)
            throw new ArgumentException("Stealth multiplier must be at least 1.0.", nameof(stealthMultiplier));

        StealthMultiplier = stealthMultiplier;
        HiddenDaggers     = 2 + level / 3;     // scales with level
        _baseAttack       = ScaleWithLevel(18, level);
    }

    // ─── Overrides ───────────────────────────────────────────────────────────

    /// <summary>
    /// Performs either a stealth strike (first attack) or a normal dagger slash.
    /// Stealth multiplier applies once; subsequent attacks are normal.
    /// </summary>
    public override int Attack()
    {
        if (IsDefeated)
        {
            Console.WriteLine($"{Name} is defeated and cannot attack.");
            return 0;
        }

        if (_stealthed)
        {
            _stealthed = false;
            int stealthDmg = (int)(_baseAttack * StealthMultiplier);
            Console.WriteLine(
                $"  🗡  {Name} strikes from the shadows! Deals {stealthDmg} stealth damage. " +
                $"(×{StealthMultiplier} multiplier)");
            return stealthDmg;
        }

        int damage = _baseAttack;
        Console.WriteLine($"  🗡  {Name} slashes! Deals {damage} damage.");
        return damage;
    }

    /// <summary>
    /// Throws all hidden daggers at once, returning total damage spread across targets.
    /// Consumes all daggers.
    /// </summary>
    /// <returns>Array of individual dagger hit values.</returns>
    public int[] ThrowDaggers()
    {
        if (HiddenDaggers <= 0)
        {
            Console.WriteLine($"{Name} has no hidden daggers left!");
            return Array.Empty<int>();
        }

        int[] hits = new int[HiddenDaggers];
        int total  = 0;

        for (int i = 0; i < HiddenDaggers; i++)
        {
            hits[i] = _rng.Next(8, 15);
            total  += hits[i];
        }

        Console.WriteLine($"  🗡🗡  {Name} throws {HiddenDaggers} daggers! Total: {total} damage.");
        HiddenDaggers = 0;
        return hits;
    }

    /// <summary>Returns the full formatted description for a Rogue.</summary>
    public override string Describe()
    {
        return $"{BaseDescriptionHeader()} | Stealth ×{StealthMultiplier} | " +
               $"Daggers: {HiddenDaggers}";
    }

    /// <summary>Rogues act first due to superior agility.</summary>
    public override int Initiative => Level * 7;
}
