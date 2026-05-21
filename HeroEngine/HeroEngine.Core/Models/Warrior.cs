namespace HeroEngine.Core.Models;

/// <summary>
/// A Warrior hero. Specialises in physical damage, possesses an <see cref="Armor"/>
/// value that absorbs a portion of incoming damage, and delivers powerful battle cries.
/// </summary>
public sealed class Warrior : Hero
{
    // ─── Warrior-specific stats ──────────────────────────────────────────────

    /// <summary>Flat damage reduction applied to every incoming hit.</summary>
    public int Armor { get; private set; }

    /// <summary>Battle cry flavour text shown on the hero's presentation.</summary>
    public string BattleCry { get; private set; }

    /// <summary>Base attack damage before crit calculation.</summary>
    private readonly int _baseAttack;

    private static readonly Random _rng = new();

    // ─── Constructor ─────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new Warrior, scaling all stats to <paramref name="level"/>.
    /// Chains to <see cref="Hero(string,int,int)"/> via <c>base()</c>.
    /// </summary>
    /// <param name="name">Warrior's name.</param>
    /// <param name="level">Starting level (≥ 1).</param>
    /// <param name="battleCry">Battle cry text displayed on presentation.</param>
    public Warrior(string name, int level, string battleCry = "For Bytecroft!")
        : base(name, level, baseHp: 100)
    {
        if (string.IsNullOrWhiteSpace(battleCry))
            throw new ArgumentException("Battle cry cannot be empty.", nameof(battleCry));

        _baseAttack = ScaleWithLevel(20, level);
        Armor       = ScaleWithLevel(10, level);
        BattleCry   = battleCry;
    }

    // ─── Overrides ───────────────────────────────────────────────────────────

    /// <summary>
    /// Performs a physical attack. Has a 25% chance to land a critical hit (2× damage).
    /// </summary>
    public override int Attack()
    {
        if (IsDefeated)
        {
            Console.WriteLine($"{Name} is defeated and cannot attack.");
            return 0;
        }

        bool isCrit = _rng.NextDouble() < 0.25;
        int damage  = isCrit ? _baseAttack * 2 : _baseAttack;

        Console.WriteLine(isCrit
            ? $"  ⚔  {Name} attacks! Deals {damage} damage. (Critical hit!)"
            : $"  ⚔  {Name} attacks! Deals {damage} damage.");

        return damage;
    }

    /// <summary>
    /// Applies armor reduction before delegating to base damage logic.
    /// </summary>
    public override int ReceiveDamage(int damage)
    {
        if (IsDefeated) return 0;

        int absorbed  = Math.Min(Armor, damage);
        int netDamage = damage - absorbed;

        Console.WriteLine(
            $"  🛡  {Name} receives {damage} damage -> absorbed {absorbed} by armor " +
            $"-> net damage: {netDamage} | HP: {Math.Max(0, CurrentHp - netDamage)}/{MaxHp}");

        return base.ReceiveDamage(netDamage);
    }

    /// <summary>Returns the full formatted description for a Warrior.</summary>
    public override string Describe()
    {
        return $"{BaseDescriptionHeader()} | Armor: {Armor}\n" +
               $"  Battle Cry: '{BattleCry}'";
    }

    /// <summary>Warriors act slightly earlier due to high initiative.</summary>
    public override int Initiative => Level * 6 + Armor / 2;
}
