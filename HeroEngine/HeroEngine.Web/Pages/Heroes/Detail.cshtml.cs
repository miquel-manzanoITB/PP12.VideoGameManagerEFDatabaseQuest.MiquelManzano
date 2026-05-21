using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DetailModel : PageModel
{
    private readonly HeroEngineContext _context;
    public HeroEntity? Hero { get; set; }

    public DetailModel(HeroEngineContext context) => _context = context;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Tasca 8.1: carrega heroi amb classe i habilitats via .Include()
        Hero = await _context.Heroes
            .Include(h => h.HeroClass)
            .Include(h => h.Abilities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (Hero == null) return NotFound();
        return Page();
    }
}