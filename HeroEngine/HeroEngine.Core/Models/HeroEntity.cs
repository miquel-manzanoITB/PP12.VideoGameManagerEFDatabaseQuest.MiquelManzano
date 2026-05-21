using HeroEngine.Core.Abilities;
using System.ComponentModel.DataAnnotations;

namespace HeroEngine.Core.Models;

/// <summary>
/// EF Core entity representing a hero stored in the database.
/// Separate from the abstract Hero used in the combat engine.
/// </summary>
public class HeroEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nom és obligatori")]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "El nivell ha de ser entre 1 i 100")]
    public int Level { get; set; } = 1;

    [Range(1, 9999)]
    public int MaxHp { get; set; }

    public int Armor { get; set; }

    public string? Biography { get; set; }

    // Clau foràna
    public int HeroClassId { get; set; }

    // Propietats de navegació (NO es guarden com a columnes)
    public HeroClass? HeroClass { get; set; }
    public ICollection<Ability> Abilities { get; set; } = new List<Ability>();
}
