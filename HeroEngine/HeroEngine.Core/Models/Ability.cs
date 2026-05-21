using System.ComponentModel.DataAnnotations;

namespace HeroEngine.Core.Models;

/// <summary>
/// EF Core entity representing an ability stored in the database.
/// Separate from the IAbility/BaseAbility used in the combat engine.
/// </summary>
public class Ability
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nom de l'habilitat és obligatori")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;   // "Attack", "Defense", "Healing", "Support"

    [Required]
    public string Rarity { get; set; } = string.Empty; // "Common", "Rare", "Epic", "Legendary"

    [Range(0, 999)]
    public int Cost { get; set; }

    // Clau foràna
    public int HeroId { get; set; }

    // Propietat de navegació
    public HeroEntity? Hero { get; set; }
}
