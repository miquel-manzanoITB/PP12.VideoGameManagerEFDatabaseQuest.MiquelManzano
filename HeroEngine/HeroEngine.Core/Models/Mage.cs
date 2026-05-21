namespace HeroEngine.Core.Models;

/// <summary>
/// A Mage hero. Casts powerful spells at the cost of mana.
/// Arcane level amplifies spell potency.
/// </summary>
public sealed class Mage : Hero
{
    // ─── Mage-specific stats ─────────────────────────────────────────────────

    /// <summary>Current mana pool consumed when casting spells.</summary>
    public int CurrentMana { get; private set; }

    /// <summary>Maximum mana, scaled with level.</summary>
    public int MaxMana { get; private set; }

    /// <summary>Arcane level multiplier applied to spell damage (1–5).</summary>
    public int ArcaneLevel { get; private set; }

    private readonly int _baseSpellDamage;
    private const int SpellManaCost = 15;

    // ─── Constructor ─────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a Mage, scaling mana and spell power to <paramref name="level"/>.
    /// </summary>
    /// <param name="name">Mage's name.</param>
    /// <param name="level">Starting level (≥ 1).</param>
    /// <param name="arcaneLevel">Arcane mastery level (1–5).</param>
    public Mage(string name, int level, int arcaneLevel = 1)
        : base(name, level, baseHp: 70)
    {
        if (arcaneLevel < 1 || arcaneLevel > 5)
            throw new ArgumentException("Arcane level must be between 1 and 5.", nameof(arcaneLevel));

        ArcaneLevel      = arcaneLevel;
        MaxMana          = ScaleWithLevel(80, level);
        CurrentMana      = MaxMana;
        _baseSpellDamage = ScaleWithLevel(15, level);
    }

    // ─── Overrides ───────────────────────────────────────────────────────────

    /// <summary>
    /// Casts a spell consuming mana. If mana is depleted falls back to a weak staff hit.
    /// </summary>
    public override int Attack()
    {
        if (IsDefeated)
        {
            Console.WriteLine($"{Name} is defeated and cannot attack.");
            return 0;
        }

        if (CurrentMana >= SpellManaCost)
        {
            CurrentMana -= SpellManaCost;
            int damage = _baseSpellDamage * ArcaneLevel;
            Console.WriteLine(
                $"  🔮  {Name} casts a spell! Deals {damage} arcane damage. " +
                $"(Mana: {CurrentMana}/{MaxMana})");
            return damage;
        }

        // OOM fallback
        int staffDmg = _baseSpellDamage / 3;
        Console.WriteLine($"  🪄  {Name} is out of mana! Staff hit for {staffDmg} damage.");
        return staffDmg;
    }

    /// <summary>Returns the full formatted description for a Mage.</summary>
    public override string Describe()
    {
        return $"{BaseDescriptionHeader()} | Mana: {CurrentMana}/{MaxMana} | Arcane Level: {ArcaneLevel}";
    }

    /// <summary>Mages act based on arcane level to reflect magical reflexes.</summary>
    public override int Initiative => Level * 4 + ArcaneLevel * 3;
}
