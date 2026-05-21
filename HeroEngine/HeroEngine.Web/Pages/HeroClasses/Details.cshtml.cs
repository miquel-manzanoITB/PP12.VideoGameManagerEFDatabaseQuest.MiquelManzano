using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class HeroClassDetailsModel : PageModel
{
    private readonly HeroEngineContext _context;
    public HeroClass? HeroClass { get; set; }

    public HeroClassDetailsModel(HeroEngineContext context) => _context = context;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        HeroClass = await _context.HeroClasses
            .Include(hc => hc.Heroes)
            .FirstOrDefaultAsync(hc => hc.Id == id);

        if (HeroClass == null) return NotFound();
        return Page();
    }
}
