using HeroEngine.Core.Models;

namespace HeroEngine.Core.Abilities;

/// <summary>
/// Helper class with a single responsibility: formatting and displaying
/// ability loadout information. Follows the Single Responsibility Principle (SRP).
/// </summary>
public static class AbilityHelper
{
    private const int LineWidth = 47;

    /// <summary>
    /// Prints the full ability loadout for a hero, sorted by rarity (Legendary first).
    /// </summary>
    /// <param name="hero">The hero whose loadout to display.</param>
    public static void PrintLoadout(Hero hero)
    {
        string title = $" {hero.Name.ToUpper()}'S ABILITY LOADOUT";
        Console.WriteLine(new string('=', LineWidth));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', LineWidth));

        var sorted = hero.GetAbilitiesByRarity().ToList();

        if (sorted.Count == 0)
        {
            Console.WriteLine("  (no abilities equipped)");
        }
        else
        {
            foreach (var ability in sorted)
            {
                Console.WriteLine(FormatAbilityLine(ability));
            }
        }

        Console.WriteLine(new string('=', LineWidth));
    }

    /// <summary>
    /// Formats a single ability as a display line matching the spec:
    /// "[LEGENDARY] Thunder Smash | Type: Attack | Cost: 40 mana"
    /// </summary>
    public static string FormatAbilityLine(IAbility ability)
    {
        string tag  = $"[{ability.Rarity.ToString().ToUpper()}]";
        string type = $"Type: {ability.Type}";
        string cost = $"Cost: {ability.Cost} mana";
        return $"  {tag,-12} {ability.Name,-16} | {type,-16} | {cost}";
    }

    /// <summary>
    /// Prints a summary count of abilities grouped by type for a hero.
    /// </summary>
    public static void PrintTypeSummary(Hero hero)
    {
        var groups = hero.Abilities
            .GroupBy(a => a.Type)
            .OrderBy(g => g.Key.ToString());

        Console.WriteLine($"\n  Ability type summary for {hero.Name}:");
        foreach (var group in groups)
            Console.WriteLine($"    {group.Key}: {group.Count()} ability/abilities");
    }
}
