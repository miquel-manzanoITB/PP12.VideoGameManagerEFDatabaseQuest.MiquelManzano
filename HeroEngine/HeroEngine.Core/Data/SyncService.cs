using HeroEngine.Core.Models;
using HeroEngine.Web.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HeroEngine.Core.Data;

/// <summary>
/// Handles bidirectional synchronisation between JSON files and the database.
/// </summary>
public class SyncService
{
    private readonly HeroEngineContext _context;
    private readonly HeroRepository _repo;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public SyncService(HeroEngineContext context, HeroRepository repo)
    {
        _context = context;
        _repo = repo;
    }

    // ─── DB → Files ──────────────────────────────────────────────────────────

    /// <summary>
    /// Exports all heroes from the database to heroes.json.
    /// Overwrites the existing file.
    /// </summary>
    public async Task<string> ExportDbToFilesAsync()
    {
        var heroes = await _context.Heroes
            .Include(h => h.HeroClass)
            .Include(h => h.Abilities)
            .ToListAsync();

        var dtos = heroes.Select(h => new HeroDto
        {
            Name = h.Name,
            Type = h.HeroClass?.Name ?? "Warrior",
            Level = h.Level,
            MaxHp = h.MaxHp,
            Armor = h.Armor,
            Abilities = h.Abilities.Select(a => new AbilityDto
            {
                Name = a.Name,
                Type = a.Type,
                Rarity = a.Rarity,
                Cost = a.Cost
            }).ToList()
        }).ToList();

        _repo.SaveAll(dtos);
        return $"Exportats {dtos.Count} heroi(s) de la BD cap a heroes.json.";
    }

    // ─── Files → DB ──────────────────────────────────────────────────────────

    /// <summary>
    /// Imports heroes from heroes.json into the database.
    /// Heroes that already exist (by name) are updated; new ones are inserted.
    /// </summary>
    public async Task<string> ImportFilesToDbAsync()
    {
        var dtos = _repo.LoadAll();
        if (!dtos.Any())
            return "El fitxer heroes.json és buit o no existeix.";

        // Ensure HeroClasses exist
        var classNames = dtos.Select(d => d.Type).Distinct().ToList();
        foreach (var className in classNames)
        {
            if (!await _context.HeroClasses.AnyAsync(hc => hc.Name == className))
            {
                _context.HeroClasses.Add(new HeroClass
                {
                    Name = className,
                    Description = className switch
                    {
                        "Warrior" => "Tanc melee d'alta defensa",
                        "Mage" => "Mag d'atac màgic",
                        "Rogue" => "Assassí ràpid i sigilós",
                        _ => ""
                    }
                });
            }
        }
        await _context.SaveChangesAsync();

        int added = 0, updated = 0;

        foreach (var dto in dtos)
        {
            var heroClass = await _context.HeroClasses
                .FirstOrDefaultAsync(hc => hc.Name == dto.Type);
            if (heroClass == null) continue;

            var existing = await _context.Heroes
                .Include(h => h.Abilities)
                .FirstOrDefaultAsync(h => h.Name == dto.Name);

            if (existing == null)
            {
                // Insert new hero
                var hero = new HeroEntity
                {
                    Name = dto.Name,
                    Level = dto.Level,
                    MaxHp = dto.MaxHp,
                    Armor = dto.Armor,
                    HeroClassId = heroClass.Id,
                    Abilities = dto.Abilities.Select(a => new Ability
                    {
                        Name = a.Name,
                        Type = a.Type,
                        Rarity = a.Rarity,
                        Cost = a.Cost
                    }).ToList()
                };
                _context.Heroes.Add(hero);
                added++;
            }
            else
            {
                // Update existing hero
                existing.Level = dto.Level;
                existing.MaxHp = dto.MaxHp;
                existing.Armor = dto.Armor;
                existing.HeroClassId = heroClass.Id;

                // Sync abilities: remove old, add new
                _context.Abilities.RemoveRange(existing.Abilities);
                existing.Abilities = dto.Abilities.Select(a => new Ability
                {
                    Name = a.Name,
                    Type = a.Type,
                    Rarity = a.Rarity,
                    Cost = a.Cost,
                    HeroId = existing.Id
                }).ToList();

                updated++;
            }
        }

        await _context.SaveChangesAsync();
        return $"Importats {added} heroi(s) nous i actualitzats {updated} des de heroes.json.";
    }
}
