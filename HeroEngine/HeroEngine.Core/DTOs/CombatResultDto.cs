public class CombatResultDto
{
    public DateTime Date { get; set; } = DateTime.Now;
    public List<string> Heroes { get; set; } = new();
    public List<string> Enemies { get; set; } = new();
    public bool Victory { get; set; }
    public int Rounds { get; set; }
    public int TotalDamage { get; set; }
    public string MostEffective { get; set; } = "";
}