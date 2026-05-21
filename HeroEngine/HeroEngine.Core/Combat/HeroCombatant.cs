using HeroEngine.Core.Models;

namespace HeroEngine.Core.Combat;

/// <summary>
/// Adapter that wraps a <see cref="Hero"/> so it can participate in combat
/// via the <see cref="ICombatant"/> interface alongside <see cref="Enemy"/> instances.
/// This avoids forcing Hero to depend on the Combat namespace (Dependency Inversion).
/// </summary>
public sealed class HeroCombatant : ICombatant
{
    private readonly Hero _hero;

    /// <summary>Creates an adapter for the given hero.</summary>
    public HeroCombatant(Hero hero)
    {
        _hero = hero ?? throw new ArgumentNullException(nameof(hero));
    }

    /// <inheritdoc/>
    public string Name        => _hero.Name;

    /// <inheritdoc/>
    public bool   IsDefeated  => _hero.IsDefeated;

    /// <inheritdoc/>
    public int    Initiative   => _hero.Initiative;

    /// <inheritdoc/>
    public int Attack()              => _hero.Attack();

    /// <inheritdoc/>
    public int ReceiveDamage(int d)  => _hero.ReceiveDamage(d);

    /// <summary>Provides access to the underlying Hero for logging purposes.</summary>
    public Hero UnderlyingHero => _hero;
}
