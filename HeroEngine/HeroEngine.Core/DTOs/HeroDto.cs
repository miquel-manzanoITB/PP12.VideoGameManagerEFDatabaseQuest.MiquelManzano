namespace HeroEngine.Web.DTOs
{
    public class HeroDto
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int Armor { get; set; }
        public List<AbilityDto> Abilities { get; set; } = new();
    }
}
