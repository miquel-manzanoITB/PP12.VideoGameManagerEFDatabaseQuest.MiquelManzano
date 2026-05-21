using HeroEngine.Core.Models;
using HeroEngine.Web.DTOs;
using System.Collections;
using System.Text.Json;

namespace HeroEngine.Core.Data;

public class HeroRepository
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _opts = new()
    { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public HeroRepository(string filePath) => _filePath = filePath;

    public List<HeroDto> LoadAll()
    {
        if (!File.Exists(_filePath)) return new();
        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<HeroDto>>(json, _opts) ?? new();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading heroes: {ex.Message}");
            return new();
        }
    }

    public void SaveAll(IEnumerable heroes)
    {
        try
        {
            string json = JsonSerializer.Serialize(heroes, _opts);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving heroes: {ex.Message}");
        }
    }

    public void Add(HeroDto hero)
    {
        var list = LoadAll();
        list.Add(hero);
        SaveAll(list);
    }

    public void Delete(string name)
    {
        var list = LoadAll();
        list.RemoveAll(h =>
            h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        SaveAll(list);
    }
}