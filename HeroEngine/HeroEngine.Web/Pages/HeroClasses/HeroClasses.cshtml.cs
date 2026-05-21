using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class HeroClassesModel : PageModel
{
    private readonly HeroEngineContext _context;
    public List<HeroClass> HeroClasses { get; set; } = new();

    public HeroClassesModel(HeroEngineContext context) => _context = context;

    public async Task OnGetAsync()
    {
        HeroClasses = await _context.HeroClasses
            .Include(hc => hc.Heroes)
            .ToListAsync();
    }

    // Tasca 8.3: Bloqueig d'eliminació si té herois associats
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var heroClass = await _context.HeroClasses
            .Include(hc => hc.Heroes)
            .FirstOrDefaultAsync(hc => hc.Id == id);

        if (heroClass == null) return NotFound();

        if (heroClass.Heroes.Any())
        {
            ModelState.AddModelError("",
                $"No es pot eliminar '{heroClass.Name}': té {heroClass.Heroes.Count} heroi(s) associat(s).");
            HeroClasses = await _context.HeroClasses.Include(hc => hc.Heroes).ToListAsync();
            return Page();
        }

        _context.HeroClasses.Remove(heroClass);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
