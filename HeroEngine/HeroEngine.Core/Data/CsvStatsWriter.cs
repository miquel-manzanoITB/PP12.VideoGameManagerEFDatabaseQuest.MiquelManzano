namespace HeroEngine.Core.Data;

public class CsvStatsWriter
{
    private readonly string _filePath;

    public CsvStatsWriter(string filePath) => _filePath = filePath;

    public void AppendCombatStats(CombatResultDto result)
    {
        bool exists = File.Exists(_filePath);
        try
        {
            using var sw = new StreamWriter(_filePath, append: true);
            if (!exists)
                sw.WriteLine(
                  "Date,Heroes,Enemies,Result,Rounds,TotalDamage,MostEffective");

            sw.WriteLine(string.Join(",",
                result.Date.ToString("yyyy-MM-dd HH:mm"),
                string.Join(";", result.Heroes),
                string.Join(";", result.Enemies),
                result.Victory ? "Victory" : "Defeat",
                result.Rounds,
                result.TotalDamage,
                result.MostEffective));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error writing CSV: {ex.Message}");
        }
    }

    public List<string[]> ReadLast(int n = 10)
    {
        if (!File.Exists(_filePath)) return new();
        try
        {
            var lines = File.ReadAllLines(_filePath)
                            .Skip(1)  // saltar cabecera
                            .TakeLast(n)
                            .Select(l => l.Split(','))
                            .ToList();
            return lines;
        }
        catch { return new(); }
    }
}