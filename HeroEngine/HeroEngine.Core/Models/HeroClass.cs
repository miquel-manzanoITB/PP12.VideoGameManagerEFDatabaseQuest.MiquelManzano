using System.ComponentModel.DataAnnotations;

namespace HeroEngine.Core.Models;

public class HeroClass
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nom de la classe és obligatori")]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty; // "Warrior", "Mage", "Rogue"

    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    // Navegació: una classe té molts herois
    public ICollection<HeroEntity> Heroes { get; set; } = new List<HeroEntity>();
}
