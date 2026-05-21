using System.Xml.Linq;

namespace HeroEngine.Core.Data;

public class GameConfig
{
    public double LevelMultiplier { get; set; } = 1.15;
    public double CriticalHitChance { get; set; } = 0.20;
    public int MaxCombatRounds { get; set; } = 20;
    public int MaxHeroesPerBattle { get; set; } = 4;

    public static GameConfig Load(string path)
    {
        if (!File.Exists(path)) return new();
        try
        {
            var doc = XDocument.Load(path);
            var root = doc.Root!;
            return new GameConfig
            {
                LevelMultiplier = double.Parse(root.Element("LevelMultiplier")!.Value),
                CriticalHitChance = double.Parse(root.Element("CriticalHitChance")!.Value),
                MaxCombatRounds = int.Parse(root.Element("MaxCombatRounds")!.Value),
                MaxHeroesPerBattle = int.Parse(root.Element("MaxHeroesPerBattle")!.Value)
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading config: {ex.Message}");
            return new();
        }
    }

    public void Save(string path)
    {
        try
        {
            var doc = new XDocument(new XElement("GameConfig",
                new XElement("LevelMultiplier", LevelMultiplier),
                new XElement("CriticalHitChance", CriticalHitChance),
                new XElement("MaxCombatRounds", MaxCombatRounds),
                new XElement("MaxHeroesPerBattle", MaxHeroesPerBattle)));
            doc.Save(path);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving config: {ex.Message}");
        }
    }
}