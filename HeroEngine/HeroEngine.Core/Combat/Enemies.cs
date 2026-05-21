namespace HeroEngine.Core.Combat;

// ─── ICombatant interface ─────────────────────────────────────────────────────

/// <summary>
/// Common contract for any entity (hero or enemy) that can participate in combat.
/// Enables polymorphic battle loops without if/switch on concrete types.
/// </summary>
public interface ICombatant
{
    /// <summary>Combatant's display name.</summary>
    string Name { get; }

    /// <summary>Whether this combatant has been defeated.</summary>
    bool IsDefeated { get; }

    /// <summary>Initiative value — higher acts first in a round.</summary>
    int Initiative { get; }

    /// <summary>Performs an attack and returns the damage dealt.</summary>
    int Attack();

    /// <summary>Applies damage and returns net damage taken.</summary>
    int ReceiveDamage(int damage);
}

// ─── Abstract Enemy base ──────────────────────────────────────────────────────

/// <summary>
/// Abstract base for all Bug Primordial enemies.
/// Mirrors the Hero hierarchy: shared state + polymorphic behaviour.
/// </summary>
public abstract class Enemy : ICombatant
{
    /// <inheritdoc/>
    public string Name { get; protected set; }

    /// <summary>Current HP of the enemy.</summary>
    public int CurrentHp { get; protected set; }

    /// <summary>Maximum HP of the enemy.</summary>
    public int MaxHp { get; protected set; }

    /// <inheritdoc/>
    public bool IsDefeated => CurrentHp <= 0;

    /// <inheritdoc/>
    public abstract int Initiative { get; }

    /// <summary>
    /// Initialises an enemy with a name and HP value.
    /// </summary>
    protected Enemy(string name, int hp)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Enemy name cannot be empty.", nameof(name));
        if (hp <= 0)
            throw new ArgumentException("HP must be positive.", nameof(hp));

        Name      = name;
        MaxHp     = hp;
        CurrentHp = hp;
    }

    /// <inheritdoc/>
    public abstract int Attack();

    /// <inheritdoc/>
    public virtual int ReceiveDamage(int damage)
    {
        if (IsDefeated) return 0;

        int net = Math.Max(0, damage);
        CurrentHp = Math.Max(0, CurrentHp - net);

        if (IsDefeated)
            Console.WriteLine($"  💀  {Name} has been defeated!");

        return net;
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{GetType().Name}] {Name} | HP: {CurrentHp}/{MaxHp}";
}

// ─── Concrete enemies ─────────────────────────────────────────────────────────

/// <summary>
/// A Minion enemy — weak, numerous, fast.
/// </summary>
public sealed class Minion : Enemy
{
    private static readonly Random _rng = new();

    /// <summary>Creates a Minion with the given name.</summary>
    public Minion(string name) : base(name, hp: 40) { }

    /// <inheritdoc/>
    public override int Initiative => 8;   // fast but fragile

    /// <inheritdoc/>
    public override int Attack()
    {
        if (IsDefeated) return 0;
        int dmg = _rng.Next(5, 12);
        Console.WriteLine($"  👾  {Name} nibbles! Deals {dmg} damage.");
        return dmg;
    }
}

/// <summary>
/// An Elite enemy — tougher and hits harder than a Minion.
/// </summary>
public sealed class Elite : Enemy
{
    private static readonly Random _rng = new();

    /// <summary>Creates an Elite enemy with the given name.</summary>
    public Elite(string name) : base(name, hp: 100) { }

    /// <inheritdoc/>
    public override int Initiative => 5;

    /// <inheritdoc/>
    public override int Attack()
    {
        if (IsDefeated) return 0;
        int dmg = _rng.Next(18, 30);
        Console.WriteLine($"  👿  {Name} lunges! Deals {dmg} damage.");
        return dmg;
    }
}

/// <summary>
/// The Boss — the Bug Primordial's most powerful agent.
/// Hits multiple heroes with its "Corrupt Memory" ability.
/// </summary>
public sealed class Boss : Enemy
{
    private static readonly Random _rng = new();

    /// <summary>Creates a Boss enemy with the given name.</summary>
    public Boss(string name) : base(name, hp: 250) { }

    /// <inheritdoc/>
    public override int Initiative => 3;   // slow but devastating

    /// <inheritdoc/>
    public override int Attack()
    {
        if (IsDefeated) return 0;
        int dmg = _rng.Next(35, 55);
        Console.WriteLine($"  🔥  {Name} uses Corrupt Memory! Deals {dmg} damage.");
        return dmg;
    }
}
