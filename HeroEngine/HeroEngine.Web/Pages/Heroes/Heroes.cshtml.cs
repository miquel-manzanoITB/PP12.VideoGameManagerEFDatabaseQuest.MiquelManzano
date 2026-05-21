using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class HeroesModel : PageModel
{
    private readonly HeroEngineContext _context;
    public List<HeroEntity> Heroes { get; set; } = new();

    public HeroesModel(HeroEngineContext context) => _context = context;

    public async Task OnGetAsync()
    {
        // Seeding: inserim dades si la BD és buida
        if (!_context.HeroClasses.Any())
        {
            var warrior = new HeroClass { Name = "Warrior", Description = "Tanc melee d'alta defensa" };
            var mage = new HeroClass { Name = "Mage", Description = "Mag d'atac màgic" };
            var rogue = new HeroClass { Name = "Rogue", Description = "Assassí ràpid i sigilós" };
            _context.HeroClasses.AddRange(warrior, mage, rogue);
            _context.SaveChanges();

            _context.Heroes.AddRange(
                new HeroEntity { Name = "Aldric", Level = 5, MaxHp = 200, Armor = 60, HeroClassId = warrior.Id },
                new HeroEntity { Name = "Lyria", Level = 4, MaxHp = 120, Armor = 20, HeroClassId = mage.Id }
            );
            _context.SaveChanges();
        }

        Heroes = await _context.Heroes
            .Include(h => h.HeroClass)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var hero = await _context.Heroes.FindAsync(id);
        if (hero != null)
        {
            _context.Heroes.Remove(hero);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
