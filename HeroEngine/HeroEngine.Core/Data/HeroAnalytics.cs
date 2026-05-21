using HeroEngine.Core.Models;
using HeroEngine.Web.DTOs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HeroEngine.Core.Data;

public class HeroAnalytics
{
    private readonly List<HeroDto> _heroes;
    public HeroAnalytics(IEnumerable<HeroDto> heroes) => _heroes = heroes.ToList();

    public IEnumerable<HeroDto> GetTopHeroesByLevel(int n)
        => _heroes.OrderByDescending(h => h.Level).Take(n);

    public IEnumerable<HeroDto> GetAbilitiesByRarity(string rarity)
        => (IEnumerable<HeroDto>)_heroes.SelectMany(h => h.Abilities)
                  .Where(a => a.Rarity.Equals(rarity,
                      StringComparison.OrdinalIgnoreCase));

    public IEnumerable<HeroDto> GetHeroesWithAbilityCount(int min)
        => _heroes.Where(h => h.Abilities.Count >= min);

    public Dictionary<string, double> GetAverageDamagePerClass()
        => _heroes.GroupBy(h => h.Type)
                  .ToDictionary(
                      g => g.Key,
                      g => g.Average(h => h.Level * 20.0));

    public IEnumerable<HeroDto> SearchHeroesByName(string pattern)
    {
        var regex = new Regex(pattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return _heroes.Where(h => regex.IsMatch(h.Name));
    }
}