namespace HeroEngine.Core.Abilities;

// ─── Abstract base (optional shared logic) ────────────────────────────────────

/// <summary>
/// Optional abstract base for abilities that share common property storage.
/// Concrete abilities may inherit from this instead of implementing <see cref="IAbility"/>
/// directly, but the system always works through the interface.
/// </summary>
public abstract class BaseAbility : IAbility
{
    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public AbilityType Type { get; }

    /// <inheritdoc/>
    public Rarity Rarity { get; }

    /// <inheritdoc/>
    public int Cost { get; }

    /// <summary>Power scalar that grows with rarity.</summary>
    protected int Power { get; }

    /// <summary>
    /// Initialises shared ability properties.
    /// Cost and Power scale automatically with rarity if not overridden.
    /// </summary>
    protected BaseAbility(string name, AbilityType type, Rarity rarity, int? cost = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ability name cannot be empty.", nameof(name));

        Name   = name;
        Type   = type;
        Rarity = rarity;
        Cost   = cost ?? RarityCost(rarity);
        Power  = RarityPower(rarity);
    }

    /// <inheritdoc/>
    public abstract int Activate(string casterName);

    // ─── Rarity helpers ──────────────────────────────────────────────────────

    private static int RarityCost(Rarity r) => r switch
    {
        Rarity.Common    => 5,
        Rarity.Rare      => 15,
        Rarity.Epic      => 25,
        Rarity.Legendary => 40,
        _                => 5
    };

    private static int RarityPower(Rarity r) => r switch
    {
        Rarity.Common    => 20,
        Rarity.Rare      => 40,
        Rarity.Epic      => 65,
        Rarity.Legendary => 95,
        _                => 10
    };
}

// ─── Concrete Abilities ───────────────────────────────────────────────────────

/// <summary>
/// [LEGENDARY] Thunder Smash — channels a massive lightning strike
/// that deals 95 damage to all enemies.
/// </summary>
public sealed class ThunderSmash : BaseAbility
{
    /// <summary>Creates a Thunder Smash ability.</summary>
    public ThunderSmash()
        : base("Thunder Smash", AbilityType.Attack, Rarity.Legendary) { }

    /// <inheritdoc/>
    public override int Activate(string casterName)
    {
        Console.WriteLine(
            $"  ⚡  Activating '{Name}' [LEGENDARY]...");
        Console.WriteLine(
            $"  ⚡  {casterName} channels the storm! {Power} lightning damage to all enemies!");
        return Power;
    }
}

/// <summary>
/// [EPIC] Iron Fortress — hardens the hero's defenses, negating the next hit.
/// </summary>
public sealed class IronFortress : BaseAbility
{
    /// <summary>Creates an Iron Fortress ability.</summary>
    public IronFortress()
        : base("Iron Fortress", AbilityType.Defense, Rarity.Epic) { }

    /// <inheritdoc/>
    public override int Activate(string casterName)
    {
        Console.WriteLine(
            $"  🏰  Activating '{Name}' [EPIC]...");
        Console.WriteLine(
            $"  🏰  {casterName} becomes a fortress of iron! Defense +{Power} for one turn.");
        return Power;
    }
}

/// <summary>
/// [RARE] Second Wind — the hero catches their breath, recovering hit points.
/// </summary>
public sealed class SecondWind : BaseAbility
{
    /// <summary>Creates a Second Wind ability.</summary>
    public SecondWind()
        : base("Second Wind", AbilityType.Healing, Rarity.Rare) { }

    /// <inheritdoc/>
    public override int Activate(string casterName)
    {
        Console.WriteLine(
            $"  💚  Activating '{Name}' [RARE]...");
        Console.WriteLine(
            $"  💚  {casterName} catches a second wind! Restores {Power} HP.");
        return Power;
    }
}

/// <summary>
/// [COMMON] War Taunt — the hero taunts the enemy, drawing their attention.
/// </summary>
public sealed class WarTaunt : BaseAbility
{
    /// <summary>Creates a War Taunt ability.</summary>
    public WarTaunt()
        : base("War Taunt", AbilityType.Support, Rarity.Common) { }

    /// <inheritdoc/>
    public override int Activate(string casterName)
    {
        Console.WriteLine(
            $"  📣  Activating '{Name}' [COMMON]...");
        Console.WriteLine(
            $"  📣  {casterName} lets out a mighty war taunt! Enemies are drawn to attack {casterName}.");
        return Power;
    }
}

/// <summary>
/// [EPIC] Shadow Strike — a backstab delivered from shadow for massive damage.
/// </summary>
public sealed class ShadowStrike : BaseAbility
{
    /// <summary>Creates a Shadow Strike ability.</summary>
    public ShadowStrike()
        : base("Shadow Strike", AbilityType.Attack, Rarity.Epic) { }

    /// <inheritdoc/>
    public override int Activate(string casterName)
    {
        Console.WriteLine(
            $"  🌑  Activating '{Name}' [EPIC]...");
        Console.WriteLine(
            $"  🌑  {casterName} melts into shadow and strikes! {Power} damage from the dark.");
        return Power;
    }
}

/// <summary>
/// [RARE] Arcane Barrier — a translucent shield of pure mana absorbs incoming damage.
/// </summary>
public sealed class ArcaneBarrier : BaseAbility
{
    /// <summary>Creates an Arcane Barrier ability.</summary>
    public ArcaneBarrier()
        : base("Arcane Barrier", AbilityType.Defense, Rarity.Rare) { }

    /// <inheritdoc/>
    public override int Activate(string casterName)
    {
        Console.WriteLine(
            $"  🔵  Activating '{Name}' [RARE]...");
        Console.WriteLine(
            $"  🔵  {casterName} erects an arcane barrier! Absorbs up to {Power} damage.");
        return Power;
    }
}
