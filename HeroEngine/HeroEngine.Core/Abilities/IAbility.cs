namespace HeroEngine.Core.Abilities;

// ─── Enums ────────────────────────────────────────────────────────────────────

/// <summary>
/// Classifies an ability by its functional role in combat.
/// </summary>
public enum AbilityType
{
    /// <summary>Deals damage to enemies.</summary>
    Attack,
    /// <summary>Reduces or absorbs incoming damage.</summary>
    Defense,
    /// <summary>Restores hit points.</summary>
    Healing,
    /// <summary>Applies buffs, debuffs, or utility effects.</summary>
    Support
}

/// <summary>
/// Classifies an ability by its rarity tier.
/// Higher numeric value = rarer (used for sorting Legendary → Common).
/// </summary>
public enum Rarity
{
    Common    = 0,
    Rare      = 1,
    Epic      = 2,
    Legendary = 3
}

// ─── IAbility interface ───────────────────────────────────────────────────────

/// <summary>
/// Contract that every ability in the HeroEngine system must fulfil.
/// Using an interface (rather than a base class) allows future abilities to
/// inherit from other hierarchies while still being treated polymorphically
/// in collections — satisfying the Open/Closed Principle (SOLID).
/// </summary>
public interface IAbility
{
    /// <summary>Unique display name of the ability.</summary>
    string Name { get; }

    /// <summary>Functional type of the ability.</summary>
    AbilityType Type { get; }

    /// <summary>Rarity tier affecting cost and potency.</summary>
    Rarity Rarity { get; }

    /// <summary>Resource cost (mana / energy / stamina) to activate.</summary>
    int Cost { get; }

    /// <summary>
    /// Activates the ability, printing a flavour message and returning the
    /// numeric effect (damage dealt, HP restored, etc.).
    /// </summary>
    /// <param name="casterName">Name of the hero casting the ability.</param>
    /// <returns>Numeric effect value (≥ 0).</returns>
    int Activate(string casterName);
}
