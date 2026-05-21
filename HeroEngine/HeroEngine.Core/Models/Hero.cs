using HeroEngine.Core.Abilities;

namespace HeroEngine.Core.Models;

/// <summary>
/// Represents the abstract base class for all heroes in the kingdom of Bytecroft.
/// Provides core attributes (Name, Level, HP) and defines the polymorphic contract
/// that all hero types must implement.
/// </summary>
public abstract class Hero
{
    // ─── Core properties ────────────────────────────────────────────────────

    /// <summary>The hero's display name.</summary>
    public string Name { get; protected set; }

    /// <summary>The hero's current level (1-based).</summary>
    public int Level { get; protected set; }

    /// <summary>The hero's maximum hit points, scaled by level.</summary>
    public int MaxHp { get; protected set; }

    /// <summary>The hero's current hit points.</summary>
    public int CurrentHp { get; protected set; }

    /// <summary>Collection of abilities equipped by this hero.</summary>
    public List<IAbility> Abilities { get; } = new();

    // ─── Constructor chaining ────────────────────────────────────────────────

    /// <summary>
    /// Initialises a hero, scaling base stats to the given level.
    /// All derived constructors must chain to this via <c>base()</c>.
    /// </summary>
    /// <param name="name">The hero's name. Must be non-empty.</param>
    /// <param name="level">The hero's starting level (minimum 1).</param>
    /// <param name="baseHp">Base HP at level 1 before scaling.</param>
    /// <exception cref="ArgumentException">Thrown when name is null/empty or level &lt; 1.</exception>
    protected Hero(string name, int level, int baseHp)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Hero name cannot be empty.", nameof(name));
        if (level < 1)
            throw new ArgumentException("Level must be at least 1.", nameof(level));
        if (baseHp <= 0)
            throw new ArgumentException("Base HP must be positive.", nameof(baseHp));

        Name    = name;
        Level   = level;
        MaxHp   = ScaleWithLevel(baseHp, level);
        CurrentHp = MaxHp;
    }

    // ─── Abstract contract ───────────────────────────────────────────────────

    /// <summary>Performs a basic attack and returns the damage dealt.</summary>
    /// <returns>Damage value (always ≥ 0).</returns>
    public abstract int Attack();

    /// <summary>Returns a formatted presentation string for this hero.</summary>
    public abstract string Describe();

    // ─── Shared behaviour ────────────────────────────────────────────────────

    /// <summary>
    /// Applies incoming damage, respecting any subclass-specific reductions.
    /// A defeated hero ignores further damage.
    /// </summary>
    /// <param name="damage">Raw incoming damage (must be ≥ 0).</param>
    /// <returns>Net damage actually applied after reductions.</returns>
    public virtual int ReceiveDamage(int damage)
    {
        if (IsDefeated)
        {
            Console.WriteLine($"{Name} is already defeated and cannot take more damage.");
            return 0;
        }
        if (damage < 0)
            throw new ArgumentException("Damage cannot be negative.", nameof(damage));

        int netDamage = Math.Max(0, damage);
        CurrentHp = Math.Max(0, CurrentHp - netDamage);

        if (IsDefeated)
            Console.WriteLine($"  ☠  {Name} has been defeated!");

        return netDamage;
    }

    /// <summary>Returns true when the hero's HP has reached zero.</summary>
    public bool IsDefeated => CurrentHp <= 0;

    /// <summary>
    /// Returns the hero's initiative value used to determine turn order in combat.
    /// Default implementation uses Level * 5; subclasses may override.
    /// </summary>
    public virtual int Initiative => Level * 5;

    // ─── Ability management ──────────────────────────────────────────────────

    /// <summary>
    /// Adds an ability to this hero's loadout.
    /// </summary>
    /// <param name="ability">Ability to add.</param>
    /// <exception cref="InvalidOperationException">Thrown if an ability with the same name already exists.</exception>
    public void AddAbility(IAbility ability)
    {
        if (ability == null)
            throw new ArgumentNullException(nameof(ability), "Cannot add a null ability.");

        if (Abilities.Any(a => a.Name.Equals(ability.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException(
                $"Hero '{Name}' already has an ability named '{ability.Name}'.");

        Abilities.Add(ability);
    }

    /// <summary>
    /// Returns the hero's abilities sorted by rarity (Legendary → Common).
    /// </summary>
    public IEnumerable<IAbility> GetAbilitiesByRarity()
        => Abilities.OrderByDescending(a => (int)a.Rarity);

    // ─── Private helpers ─────────────────────────────────────────────────────

    /// <summary>Scales a base stat linearly with hero level.</summary>
    protected static int ScaleWithLevel(int baseStat, int level)
        => baseStat + (level - 1) * (baseStat / 5);

    /// <summary>Shared header line used by all subclasses in Describe().</summary>
    protected string BaseDescriptionHeader()
        => $"[{GetType().Name.ToUpper()}] {Name} | Level: {Level} | HP: {CurrentHp}/{MaxHp}";
}
