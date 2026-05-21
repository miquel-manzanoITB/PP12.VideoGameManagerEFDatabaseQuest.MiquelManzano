using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DeleteModel : PageModel
{
    private readonly HeroEngineContext _context;

    public DeleteModel(HeroEngineContext context) => _context = context;

    public HeroEntity? Hero { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Hero = await _context.Heroes
            .Include(h => h.HeroClass)
            .Include(h => h.Abilities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (Hero == null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var hero = await _context.Heroes
            .Include(h => h.Abilities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hero != null)
        {
            // Les Abilities s'eliminen en cascada (configurat al context)
            _context.Heroes.Remove(hero);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Heroes/Heroes");
    }
}
